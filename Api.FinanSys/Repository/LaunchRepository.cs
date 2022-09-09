
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinansysControl.Data;
using FinansysControl.Models;
using Microsoft.AspNetCore.Mvc;
using Repository.Base;

namespace Repository
{
    public class LaunchRepository : EfCoreGenericRepository<Launch, FinansysContext>
    {
        private readonly FinansysContext _context;
        public LaunchRepository(FinansysContext context) : base(context)
        {
            this._context = context;
        }

        public ActionResult<IEnumerable<Launch>> GetAllOrdered(int month, int year)
        {
           var launchs = this._context.Launch.Where(s => s.Day.Month == month || month == 0).Where(s => s.Day.Year == year || year == 0).OrderByDescending(s => s.Day).ToList();
           var launchsPredicted = launchs.Where(s => s.ValuePrev > s.ValueExec).ToList();
           launchs.RemoveAll(s => s.ValuePrev > s.ValueExec);
           launchs.InsertRange(0,launchsPredicted);

           return launchs;
        }

        public ActionResult<bool> CheckDuplicate(Launch launch)
        {
            return this._context.Launch.Any(w => w.Day == launch.Day && (w.ValueExec == launch.ValueExec || w.ValuePrev == launch.ValuePrev));
        }

        public bool CheckIfLoaded(ImportModel f)
        {
            return this._context.Launch.Any(w => w.Day == f.DateLaunch
                                            && (w.ValueExec == f.ValueLaunch || w.ValuePrev == f.ValueLaunch)
                                           );
        }

        public async Task<ImportRequest> ProcessImport(ImportRequest importRequest, int accountId)
        {
            var launchListMapped = importRequest.importModel.Select(s => new Launch {
                                                        AccountId = accountId,
                                                        Active = true,
                                                        BudgetId = s.ProspectiveBudgetId.Value,
                                                        Day = s.DateLaunch,
                                                        Description = s.Description,
                                                        TypeLaunch = s.TypeLaunch == "Cr�dito" ? "R" : "D",
                                                        DateCreated = DateTime.Now,
                                                        UserCreated = "ImportAdmin",
                                                        ValueExec =  s.ValueLaunch,
                                                        ValuePrev = 0,
            });

            this._context.Launch.AddRange(launchListMapped);

            CreateRegisterImport(launchListMapped, importRequest);

            await this._context.SaveChangesAsync();

            return importRequest;
        }

        private void CreateRegisterImport(IEnumerable<Launch> launchListMapped, ImportRequest importRequest)
        {
            var data = launchListMapped.Select(s => new ImportData {
                                                        AccountId = s.AccountId,
                                                        DateCreated = DateTime.Now,
                                                        DateLaunch = s.Day,
                                                        Description = s.Description,
                                                        ProspectiveBudgetId = s.BudgetId,
                                                        SelectedBudgetId = s.BudgetId,
                                                        ProspectiveLoaded = false,
                                                        TypeLaunch = s.TypeLaunch,
                                                        UserCreated = s.UserCreated,
                                                        ValueLaunch = s.ValueExec,
                                              }).ToList();

            var importRegister = new Import { DateCreated = DateTime.Now, FileName = importRequest.FileName, UserCreated = "ImportAdmin", ImportData = data };

            this._context.Import.AddRange(importRegister);
            this._context.SaveChanges();
        }

        public bool CheckIfFileWasImported(string fileName)
        {
            return this._context.Import.Any(w => w.FileName == fileName);
        }
    }
}