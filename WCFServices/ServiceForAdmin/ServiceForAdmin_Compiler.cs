using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;

using DataModel;
using WCFServices.Exceptions;

using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

using WCFServices.ServiceForAdmin.DTO;

namespace WCFServices.ServiceForAdmin
{
    partial class ServiceForAdmin : IServiceForAdmin
    {
        // получить один компилятор
        public CompilerDTO GetCompiler(String SessionKey, int CompilerId)
        {
            // немного страдает произволительность, но зато нет CopyPaste
            return GetCompilers(SessionKey).Where(c => c.Id == CompilerId).Single();
        }

        public IEnumerable<CompilerDTO> GetCompilers(String SessionKey)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                var tmp = from c in db.Compilers
                          select new CompilerDTO
                          {
                              Id = c.Id,
                              Note = c.Note,
                              CompilePath = c.CompilePath,
                              CompilerType = c.CompilerType,
                              Enabled = c.Enabled,
                              Extension = c.Extension,
                              FileNameSource = c.FileNameSource,
                              FileNameTarget = c.FileNameTarget,
                              FullName = c.FullName,
                              RunPath = c.RunPath,
                              ShortName = c.ShortName,
                              TestCode = c.TestCode
                          };
                return tmp.ToList();
            }
        }

        //  обновить информацию о компиляторе
        public int InsertOrUpdateCompiler(String SessionKey, CompilerDTO Compiler)
        {
            CheckSession(SessionKey);
            bool isNewRecord = (Compiler.Id < 0);

            using (var db = new DataBase())
            {
                if ((Compiler.CompilerType == "native" || Compiler.CompilerType == "javavm" ||
                    Compiler.CompilerType == "dotnet" || Compiler.CompilerType == "custom") == false)
                    throw new Exception("CompilerType Должен быть одним из: native, javavm, dotnet, custom");

                Compiler cmp;
                if (isNewRecord)
                {
                    cmp = new DataModel.Compiler();
                    cmp.CompilerData_ver = 0;
                }
                else
                    cmp = db.Compilers.Where(c => c.Id == Compiler.Id).Single();

                cmp.ShortName = Compiler.ShortName;
                cmp.FullName = Compiler.FullName;
                cmp.CompilerType = Compiler.CompilerType;
                cmp.CompilePath = Compiler.CompilePath;
                cmp.RunPath = Compiler.RunPath;
                cmp.Enabled = Compiler.Enabled;
                cmp.FileNameSource = Compiler.FileNameSource;
                cmp.FileNameTarget = Compiler.FileNameTarget;
                cmp.Note = Compiler.Note;
                cmp.Extension = Compiler.Extension;
                cmp.TestCode = Compiler.TestCode;

                cmp.CompilerData_ver++;

                if (isNewRecord)
                    db.Compilers.AddObject(cmp);

                int res = db.SaveChanges();
                if (res != 1) throw new Exception("Error insert/update operation");

                // если новая запись - то в cmp.Id поместится id этой записи в БД
                return cmp.Id;
            }
        }


        public bool DeleteCompiler(String SessionKey, int CompilerId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {

                var cmp = db.Compilers.Where(c => c.Id == CompilerId).Single();

                if (cmp.Checkers.Count() > 0 || cmp.Submits.Count() > 0)
                    throw new Exception("Невозможно удалить, т.к. существуют связанные записи(сабмиты, чекеры)");
                db.Compilers.DeleteObject(cmp);

                db.SaveChanges();

                if (db.Compilers.Where(c => c.Id == CompilerId).SingleOrDefault() != null)
                    throw new Exception("Ошибка при удалении компилятора");

                return true;
            }

        }

    }
}
