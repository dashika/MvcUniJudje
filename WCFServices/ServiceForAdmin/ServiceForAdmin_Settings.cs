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
    public partial class ServiceForAdmin : IServiceForAdmin
    {
        public IEnumerable<SettingDTO> GetAllSettings(String SessionKey)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                var res = (from a in db.Settings
                           select new SettingDTO
                           {
                               Id = a.Id,
                               Description = a.Description,
                               KeyName = a.KeyName,
                               ValueBool = a.ValueBool,
                               ValueDateTime = a.ValueDateTime,
                               ValueGuid = a.ValueGuid,
                               ValueInt = a.ValueInt,
                               ValueReal = a.ValueReal,
                               ValueString = a.ValueString
                           }).ToList();
                return res;
            }
        }

         // Удаляет настройку
        public void DeleteSetting(String SessionKey, int settingId)
        {
            CheckSession(SessionKey);
            using (var db = new DataBase())
            {
                db.Settings.DeleteObject(db.Settings.Where(s => s.Id == settingId).Single());
                db.SaveChanges();
            }
        }

        // добавляет или изменяет настройку. Определяется переданным параметром setting.id.
        // Возвращает Id измененной или созданной записи
        public int AddOrUpdateSetting(String SessionKey, SettingDTO setting)
        {
            CheckSession(SessionKey);

            using (var db = new DataBase())
            {
                if (setting.ValueString == "")
                    setting.ValueString = null;

                int notNullCount = 0;
                if (setting.ValueBool.HasValue) notNullCount++;
                if (setting.ValueDateTime.HasValue) notNullCount++;
                if (setting.ValueGuid.HasValue) notNullCount++;
                if (setting.ValueInt.HasValue) notNullCount++;
                if (setting.ValueReal.HasValue) notNullCount++;
                if (setting.ValueString != null) notNullCount++;

                if (notNullCount != 1)
                    throw new Exception("Для одного ключа может быть только одно значение. Найдено " + notNullCount + " значений");
                if (setting.KeyName == "" || setting.KeyName == null)
                    throw new Exception("Не указан key");

                if (setting.Id < 0)
                {
                    var newobj = new Setting
                    {
                        KeyName = setting.KeyName,
                        Description = setting.Description,
                        ValueBool = setting.ValueBool,
                        ValueDateTime = setting.ValueDateTime,
                        ValueGuid = setting.ValueGuid,
                        ValueInt = setting.ValueInt,
                        ValueReal = (float?)setting.ValueReal,
                        ValueString = setting.ValueString
                    };

                    db.Settings.AddObject(newobj);
                    db.SaveChanges();
                    return newobj.Id;
                }
                else
                {
                    var elem = db.Settings.Where(s => s.Id == setting.Id).Single();
                    elem.KeyName = setting.KeyName;
                    elem.Description = setting.Description;
                    elem.ValueBool = setting.ValueBool;
                    elem.ValueDateTime = setting.ValueDateTime;
                    elem.ValueGuid = setting.ValueGuid;
                    elem.ValueInt = setting.ValueInt;
                    elem.ValueReal = (float?)setting.ValueReal;
                    elem.ValueString = setting.ValueString;

                    db.SaveChanges();
                    return setting.Id;
                }

            }
        }


    }
}
