using BaseRepository;

namespace IProfile.Data
{
    public class Class1 : ClasePadre
    {
        public string invokeParent()
        {
            return base.ParentMethod();
        }
    }
}