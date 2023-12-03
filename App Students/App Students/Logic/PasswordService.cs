using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isopoh.Cryptography.Argon2;

namespace App_Students.Logic
{
    public class PasswordService
    {

        public static string HashPasswordArgon2(string psw)
        {
            return Argon2.Hash(psw);
        }

        public static bool VerifyPasswordArgon2(string password, string hashedPassword)
        {
            return Argon2.Verify(hashedPassword, password);
        }
    }
}
