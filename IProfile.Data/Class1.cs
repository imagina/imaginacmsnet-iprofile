using BaseRepository;
using IProfile.Data.Entities;

namespace IProfile.Data
{
    public class Class1 : ClasePadre
    {
        public string invokeParent()
        {
            
            using(IprofileContext db = new IprofileContext())
            {
                return db.AspNetUsers.FirstOrDefault().Email;
            }
        }
    }
}