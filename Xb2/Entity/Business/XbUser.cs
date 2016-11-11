namespace Xb2.Entity.Business
{
    public class XbUser
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }

        public override string ToString()
        {
            return string.Format("User Info: id:{0}, name:{1}, isAdmin:{2}", ID, Name, IsAdmin);
        }
    }
}
