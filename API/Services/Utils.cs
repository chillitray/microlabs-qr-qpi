using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Services
{
    public class Utils
    {
        public string GenerateRandomOTP(int flag=1)  
  
        {  

            string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            int iOTPLength = 6;
            if(flag==2){
                iOTPLength=15;
            }

        
            string sOTP = String.Empty;  
        
            string sTempChars = String.Empty;  
        
            Random rand = new Random();  
        
            for (int i = 0; i < iOTPLength; i++)  
        
            {  
        
                int p = rand.Next(0, saAllowedCharacters.Length);  
        
                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];  
        
                sOTP += sTempChars;  
        
            }  
        
            return sOTP;  
        
        }


        private static byte next(int current)
        {
            if (current == 57)
                current = 65;
            else if (current == 90)
                current = 97;
            else if (current == 122)
                current = 48;
            else
                current++;

            return Convert.ToByte(current);
        } 

        public string GenerateNextString(string id="0000000000")
        {
            var aSCIIValues = ASCIIEncoding.ASCII.GetBytes(id);

            var indexToCheck = aSCIIValues.Length - 1;
            bool keepChecking = true;
            while (keepChecking)
            {
                    aSCIIValues[indexToCheck] = next(aSCIIValues[indexToCheck]);
                    // Console.WriteLine(aSCIIValues[indexToCheck]);
                    if (aSCIIValues[indexToCheck] == 48 && indexToCheck != 0)

                        indexToCheck--;
                    else
                        keepChecking = false;
            }

           return ASCIIEncoding.ASCII.GetString(aSCIIValues);

        }
    }
}