namespace NorthwindWeb.Backend{
    class Account{
        DecTree<string> Storage;
        string Username;
        long HashedPassword;
        public Account(string username, string password, DecTree<string> storage){
            try{
                storage.Get(username);
            }catch (System.NullReferenceException)
            {
                Storage = storage;
                this.Username = username;
                this.HashedPassword = storage.Hasher(username[0], password);
                storage.Add(this.Username, this.ToString());
            }
        }

        public bool VerifyPassword(string password){
            return Storage.Hasher(Username[0], password) == HashedPassword;
        }

        
    }
}