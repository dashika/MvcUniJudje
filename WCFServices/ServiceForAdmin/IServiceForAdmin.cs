using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;

using WCFServices.ServiceForAdmin.DTO;

namespace WCFServices.ServiceForAdmin
{
    [ServiceContract]
    interface IServiceForAdmin
    {
        #region User_login   // авторизация администратора
        // авторизация пользователя
        [OperationContract]
        String LoginUser(String Login, String Password);

        # endregion User_login
        
        [OperationContract]
        int GetVersionAPI();

        // Проверка доступности БД
        [OperationContract]
        bool CheckDatabaseExist();

        // загрузить словари
        [OperationContract]
        DictionaryCollectionDTO GetDictionaries(String SessionKey, bool IncludeContests, bool IncludeTasks, bool IncludeUsers);

        #region CRUID_User   // работа с пользователями
       
        // Получить список пользователей
        [OperationContract]
        IEnumerable<UserDTO> GetUsers(String SessionKey);

        // Получить расширеную информацию о пользователе
        [OperationContract]
        UserExDTO GetUser(String SessionKey, int UserId);

        // Получить пароль пользователя
        [OperationContract]
        String GetUserPassword(String SessionKey, int UserId, String key);

        // Получить пароль пользователя
        [OperationContract]
        String GenerateNewUserPassword(String SessionKey, int UserId);

        // создать пользователя
        [OperationContract]
        UserExDTO CreateUser(String SessionKey);
        
        /// <summary>
        /// Создание пользователя
        /// </summary>
        /// <param name="SessionKey"></param>
        /// <param name="Data"></param>
        [OperationContract]
        void CreateOneUser(String SessionKey, UserExDTO Data);

         // заблокировать группу пользователей
          [OperationContract]
        void BlockGroupUser(String SessionKey, int[] id, DateTime date);

        [OperationContract]
          void UnBlockGroupUser(String SessionKey, int[] id);

        // удалить группу пользователей
          [OperationContract]
          void DeleteGroupUser(String SessionKey, int[] id);

        // обновить данные пользователя
        [OperationContract]
        void UpdateUser(String SessionKey, UserExDTO Data);

         [OperationContract]
        void WebUpdateUser(String SessionKey, UserExDTO Data, int id, int failCount, bool sentConf, bool emailConf);

        // удалить задачу
        [OperationContract]
        void DeleteUser(String SessionKey, int UserId);

        // Обновить список групп, в которых состоит пользователь
        [OperationContract]
        void UpdateGroupsForUser(String SessionKey, int UserId, IEnumerable<int> newGroupIdCollecton);

        //отправить пароль на почту
        [OperationContract]
        void SendPasswordOnEmail(String SessionKey, String pass, int id);

        // Запросить валидацию email
        [OperationContract]
        void ValidateUserEmail(String SessionKey, int UserId);

        // Запросить восстановление пароля через email
        [OperationContract]
        void ChangePasswordUsingUserEmail(String SessionKey, int UserId);
        
        #endregion CRUID_User

        #region CRUID_Group   // работа с группами
        // Получить коллекцию всех групп
        [OperationContract]
        IEnumerable<GroupDTO> GetAllGroups(String SessionKey);

        // Получить пользователь соотвествующей группы
        [OperationContract]
        IEnumerable<UserDTO> GetAllUserInGroup(String SessionKey, Int32 ID);

        /// Полечение НЕ участников группы
        [OperationContract]
        IEnumerable<UserDTO> GetAllUserOutGroup(String SessionKey, Int32 ID);

        /// Получение информации о группе
        [OperationContract]
        GroupDTO GetGroupInfo(String SessionKey, Int32 ID);

        /// Получение контестов группы
        [OperationContract]
        IEnumerable<String> GetContestsInGroup(String SessionKey, Int32 Id);

        /// Создание новой группы
         [OperationContract]
        GroupDTO CreateGroup(String SessionKey);

         /// Сохранение изменений
         [OperationContract]
        void UpdateGroup(String SessionKey, GroupDTO Group);

         [OperationContract]
         void WebUpdateGroup(String SessionKey, String GroupTitle, int id);

         /// Удаление группы
         [OperationContract]
         void DeleteGroup(String SessionKey, int GroupID);

         /// Добавление участника в группу
         [OperationContract]
        UserDTO AddInGroup(String SessionKey, int GroupID, int UserID);

         /// Извлечение участника из группы
         [OperationContract]
         void DeleteOutFromGroup(String SessionKey, int GroupID, int UserID);

        #endregion CRUID_Group   // работа с группами

        #region CRUID_Compiler   // работа с компиляторами
        // получить список компиляторов
        [OperationContract]
        IEnumerable<CompilerDTO> GetCompilers(String SessionKey);

        // получить компиляторо
        [OperationContract]
        CompilerDTO GetCompiler(String SessionKey, int CompilerId);

        // создать компилятор или обновить информацию о компиляторе
        [OperationContract]
        int InsertOrUpdateCompiler(String SessionKey, CompilerDTO Compiler);

        // удалить компилятор
        [OperationContract]
        bool DeleteCompiler(String SessionKey,int CompilerId);

        #endregion CRUID_Compiler
        
        #region CRUID_Submit   //работа с сабмитами
        // получить список сабмитов с доп. информацией
        [OperationContract]
        IEnumerable<SubmitDTO> GetSubmits(String SessionKey, int? FilterContestId, int? FilterUserId, int? FilterTaskId, int CountOnPage, int PageNum);

        // получить все сабмиты (без фильтров и првязки к странице)
        [OperationContract]
        IEnumerable<SubmitDTO> WebGetSubmits(String SessionKey);

        // получить список сабмитов,  с обновляемой при проверке информацией
        [OperationContract]
        IEnumerable<SubmitDTO> GetSubmitUpdate(String SessionKey, int[] SubmitIdCollection);
        
        [OperationContract]
        String SubmitShowCode(String SessionKey,int SubmitId);
        
        [OperationContract]
        String SubmitShowAdminLog(String SessionKey,int SubmitId);
        
        [OperationContract]
        String SubmitShowUserLog(String SessionKey,int SubmitId);

        [OperationContract]
        bool RejudgeSubmit(String SessionKey, IEnumerable<int> SubmitIdCollection);

        [OperationContract]
        bool DeleteSubmit(String SessionKey, IEnumerable<int> SubmitIdCollection);

        #endregion CRUID_Submit

        #region CRUID_Task   //работа с задачами
        // получить список задач
        [OperationContract]
        IEnumerable<TaskDTO> GetTasks(String SessionKey);

        // получить задачу
        [OperationContract]
        TaskExDTO GetTask(String SessionKey, int TaskId, bool IncludeStatement);

        [OperationContract]
        IEnumerable<TaskComlexity> GetTasksComlexity(String SessionKey);

        [OperationContract]
        IEnumerable<TaskComlexity> GetTasksSubject(String SessionKey);

        // получить текст задачи
        [OperationContract]
        String GetTaskStatement(String SessionKey, int TaskId );

        // создать задачу
        [OperationContract]
        TaskExDTO CreateTask(String SessionKey);

        [OperationContract]
        int GetSubject(String SessionKey, int ID);

        // обновить текст задачи
        [OperationContract]
        void UpdateTaskStatement(String SessionKey, int TaskId, String Statement);

         [OperationContract]
        void UpdateSubjectComlexity(String SessionKey, int  IDTask, int  subject, int Comlexity);

        // обновить pdf версию задачи
        [OperationContract]
        void UpdateTaskPdfStatement(String SessionKey, int TaskId, byte[] Statement);

        // удалить pdf версию задачи
        [OperationContract]
        void DeleteTaskPdfStatement(String SessionKey, int TaskId);

        // удалить pdf версию задачи
        [OperationContract]
        byte[] DownloadTaskPdfStatement(String SessionKey, int TaskId);


        // или обновить информацию о задаче
        [OperationContract]
        void UpdateTask(String SessionKey, TaskExDTO Task, bool UpdateStatement);

        // удалить задачу
        [OperationContract]
        void DeleteTask(String SessionKey, int TaskId);

        // обновить задачу,тесты и чекеры одним запросом(применяется для загрузки из ZIP архива)
        // в отличии от 
        [OperationContract]
        void UpdateTaskEx(String SessionKey, TaskExDTO Task, IEnumerable<TestDTO> Tests, bool AppendTests,  bool UpdateTaskStatement, IEnumerable<CheckerDTO> Checkers, bool AppendCheckers);

        /// добавление новой темы
           [OperationContract]
         void AddNewSubject(String SessionKey, String title);

           /// добавление сложности
          [OperationContract]
         void AddNewComlexity(String SessionKey, String title);

          /// удаление темы
          [OperationContract]
          void DeleteSubject(String SessionKey, Int32 IDS);

          /// удаление сложности
           [OperationContract]
         void DeleteComlexity(String SessionKey, Int32 IDD);

           ///  Обновление темы
           [OperationContract]
         void UpdateNewSubject(String SessionKey, Int32 IDS, String title);

           /// Обновление сложности
           [OperationContract]
           void UpdateNewComlexity(String SessionKey, Int32 IDD, String title);
  
         [OperationContract]
         int GetIDSubject(String SessionKey, String title);

         [OperationContract]
         int GetIDComlexity(String SessionKey, String title);

         /// Получить название темы
         [OperationContract]
        String GetTitleSubject(String SessionKey, Int32 ID);

         /// получить название сложности
         [OperationContract]
         String GetTitleComlexity(String SessionKey, Int32 ID);
        
        #endregion CRUID_Task
        
        #region CRUID_Test   //работа с тестами
        // получить тест
        [OperationContract]
        TestDTO GetTest(String SessionKey, int TestId);

        // создать тест
        [OperationContract]
        TestDTO CreateTest(String SessionKey, int TaskId);

        // обновить тест
        [OperationContract]
        void UpdateTest(String SessionKey, TestDTO Test);

        // удалить тест
        [OperationContract]
        void DeleteTest(String SessionKey, int TestId);

        #endregion CRUID_Test

        
        #region CRUID_Checker   //работа с чекерами
        // получить чекер
        [OperationContract]
        CheckerDTO GetChecker(String SessionKey, int CheckerId);

        // создать чекер
        [OperationContract]
        CheckerDTO CreateChecker(String SessionKey, int TaskId);

        // обновить чекер
        [OperationContract]
        void UpdateChecker(String SessionKey, CheckerDTO Checker);

        // удалить чекер
        [OperationContract]
        void DeleteChecker(String SessionKey, int CheckerId);

        #endregion CRUID_Checker

        #region CRUID_Contest   // работа с контестами
        // получить список контестов
        [OperationContract]
        IEnumerable<ContestDTO> GetContests(String SessionKey);

        // получить контест
        [OperationContract]
        ContestExDTO GetContest(String SessionKey, int ContestId);  
        


     

        // Обновить основную информацию о кнонетсте
        [OperationContract]
        void UpdateContest(String SessionKey, ContestExDTO Contest);

        // Обновить привязку груп к контесту
        [OperationContract]
        void UpdateGroupsForContest(String SessionKey, int ContestId, IEnumerable<GroupInContestDTO> newGroupCollecton);

        // Обновить привязку компиляторов к контесту
        [OperationContract]
        void UpdateCompilersForContest(String SessionKey, int ContestId, IEnumerable<int> newCompilerIdCollecton);

        // Обновить привязку задач к контесту
        [OperationContract]
        void UpdateTasksForContest(String SessionKey, int ContestId, IEnumerable<TaskInContestDTO> newTasksCollecton);

        // создать Контест
        [OperationContract]
        ContestExDTO CreateContest(String SessionKey);

        // удалить контест
        [OperationContract]
        void DeleteContest(String SessionKey, int ContestId);

        // Получить приветственное сообщение
        [OperationContract]
        String GetWellcomeText(String SessionKey, int ContestId);

        // Обновить приветственное сообщение
        [OperationContract]
        void UpdateWellcomeText(String SessionKey, int ContestId, String WellcomeText);

        // Получить спонсорский текст 
        [OperationContract]
        String GetSponsorText(String SessionKey, int ContestId);

        // Обновить спонсорский текст
        [OperationContract]
        void UpdateSponsorText(String SessionKey, int ContestId, String SponsorText);

        #endregion CRUID_Contest

        #region CRUID_ContestType   // работа с типами контестов
        // получить список типов контестов
        [OperationContract]
        IEnumerable<ContestTypeDTO> GetContestTypes(String SessionKey);
        
        #endregion CRUID_ContestType

        #region CRUID_LogForUser   // работа с логами пользователей
        
        // получить лог юзеров за последние 2 дня
        [OperationContract]
        IEnumerable<LogForUserDTO> GetLogForUser(String SessionKey);

        #endregion CRUID_LogForUser

        #region CRUID_Settings   // работа с настройками системы
        [OperationContract]
        IEnumerable<SettingDTO> GetAllSettings(String SessionKey);

        // Удаляет настройку
        [OperationContract]
        void DeleteSetting(String SessionKey, int settingId);
        

        // добавляет или изменяет настройку. Определяется переданным параметром setting.id.
        // Возвращает Id измененной или созданной записи
        [OperationContract]
        int AddOrUpdateSetting(String SessionKey, SettingDTO setting);

        #endregion CRUID_Settings

        #region CRUID_Admin // работа с администраторами
        /// Получить всех админов
        [OperationContract]
        IEnumerable<AdminDTO> GetAdmins(String SessionKey);

        /// Получить опреденный список в зависимости от ID
        [OperationContract]
        AdminDTO GetCollection(String SessionKey, string str);

        [OperationContract]
        AdminDTO GetAdmin(String SessionKey, int id);

        /// Получить коллекцию допусков
        [OperationContract]
        IEnumerable<AccessToPanel> GetCollectionAccess(String SessionKey, int ID);

        /// Получить коллекцию кнопок
        [OperationContract]
        IEnumerable<AccessButtons> GetCollectionButtons(String SessionKey);

        /// Генерация нового пароля админа
        [OperationContract]
        String GenerateNewAdminPassword(String SessionKey, int AdminId);

        // Генерация нового парол и отправка его по почте
        [OperationContract]
        String GetNewPasswordForAdminAndSendByEmail(String SessionKey, int id, string password);

        [OperationContract]
        String CreateNewPasswordUseble(String SessionKey, int AdminId, string pass);

        /// Создание нового администратора
        [OperationContract]
        AdminDTO CreateAdmin(String SessionKey);

        /// Сохранение изменений
        [OperationContract]
        void UpdateAdmin(String SessionKey, AdminDTO Admin);

        [OperationContract]
        void WebUpdateAdmin(String SessionKey, AdminDTO Admin, int id, bool Notify, bool acess);


        /// Удаление администратора
        [OperationContract]
        void DeleteAdmin(String SessionKey, int AdminId);

        /// Добавление/удаление доступа к панелям
        [OperationContract]
        void AccessAdmins(String SessionKey, Int32 AdminId, Boolean[] access);

        #endregion CRUID_Admin
    }
}
