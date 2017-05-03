using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFServices.ServiceForAdmin.DTO
{
    public class DictionaryCollectionDTO
    {
        public List<KeyValueDTO> Users;

        public List<KeyValueDTO> Contests;

        public List<KeyValueDTO> Tasks;
    }
}
