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

        public string GenerateNextString(string id="000000000000")
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

        private static Random rand = new Random();

        public string RandomPassword(int length = 8)
        {
            string[] categories = {
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                "abcdefghijklmnopqrstuvwxyz",
                "!-_*+&$",
                "0123456789" };

            List<char> chars = new List<char>(length);

            // add one char from each category
            foreach(string cat in categories)
            {
                chars.Add(cat[rand.Next(cat.Length)]);
            }

            // add random chars from any category until we hit the length
            string all = string.Concat(categories);            
            while (chars.Count < length)
            {
                chars.Add(all[rand.Next(all.Length)]);
            }

            // shuffle and return our password
            var shuffled = chars.OrderBy(c => rand.NextDouble()).ToArray();
            return new string(shuffled);
        }

            public string DictionaryToString(Dictionary < string, string > dictionary) {  
                string dictionaryString = "{";  
                foreach(KeyValuePair < string, string > keyValues in dictionary) {  
                dictionaryString += keyValues.Key + " : " + keyValues.Value + ", ";  
                }  
                return dictionaryString.TrimEnd(',', ' ') + "}";  
            }




        private List<char> RandQrChars = new List<char> {'3','5','6','8','0','A','B','D','E','G','J','M','N','R','T','W','X','Z','b','c','f','h','i','k','o','n','v','y'};
        // Function to encrypt the String (QR Codes)
        public String QrEncryption(String id)
        {
            var reverse = false;
            var lastChar = id[id.Length-1]; 
            if(RandQrChars.Contains(lastChar)){
                reverse = true;
            }

            //remove the last character from the id
            //since we need last character should be constant
            //we will add it after encryption
            id = id.Remove(id.Length-1); 


            char[] s = id.ToCharArray();
            int l = s.Length;
            int b = (int) Math.Ceiling(Math.Sqrt(l));
            int a = (int) Math.Floor(Math.Sqrt(l));
            String encrypted = "";
            if (b * a < l)
            {
                if (Math.Min(b, a) == b)
                {
                    b = b + 1;
                }
                else
                {
                    a = a + 1;
                }
            }
        
            // Matrix to generate the
            // Encrypted String
            

            if(reverse){
                char [,]arr = new char[b,a];
                // fill the matring by reversing the id
                int k = id.Length-1;
            
                // Fill the matrix row-wise
                for (int j = 0; j < b; j++)
                {
                    for (int i = 0; i < a; i++)
                    {
                        if (k >= 0)
                        {
                            arr[i,j] = s[k];
                        }
                        k--;
                    }
                }
                // Loop to generate
                // encrypted String
                for (int j = 0; j < b; j++)
                {
                    for (int i = 0; i < a; i++)
                    {
                        encrypted = encrypted +
                                    arr[i, j];
                    }
                }


            }
            else{
                char [,]arr = new char[a, b];
                // fill the matring without reversing the id
                int k = 0;
            
                // Fill the matrix row-wise
                for (int j = 0; j < a; j++)
                {
                    for (int i = 0; i < b; i++)
                    {
                        if (k < l)
                        {
                            arr[j, i] = s[k];
                        }
                        k++;
                    }

                    
                }

                // Loop to generate
                // encrypted String
                for (int j = 0; j < b; j++)
                {
                    for (int i = 0; i < a; i++)
                    {
                        encrypted = encrypted +
                                    arr[i, j];
                    }
                }
            }
        
            

            //add the last character at the end of encrypted id
            encrypted = encrypted+lastChar;
            return encrypted;
        }


        
        // Function to decrypt the String(QR Codes)
        public String QrDecryption(String id)
        {
            var reverse = false;
            var lastChar = id[id.Length-1]; 
            if(RandQrChars.Contains(lastChar)){
                reverse = true;
            }

            //remove the last character from the id
            //since we need last character should be constant
            //we will add it after encryption
            id = id.Remove(id.Length-1); 


            char[] s = id.ToCharArray();
            int l = s.Length;
            int b = (int) Math.Ceiling(Math.Sqrt(l));
            int a = (int) Math.Floor(Math.Sqrt(l));
            String decrypted="";
        
            // Matrix to generate the
            // Encrypted String
            
            // // Fill the matrix column-wise
            // for (int j = 0; j < b; j++)
            // {
            //     for (int i = 0; i < a; i++)
            //     {
            //         if (k < l)
            //         {
            //             arr[j, i] = s[k];
            //         }
            //         k++;
            //     }
            // }
        
            // // Loop to generate
            // // decrypted String
            // for (int j = 0; j < a; j++)
            // {
            //     for (int i = 0; i < b; i++)
            //     {
            //         decrypted = decrypted +
            //                     arr[i, j];
            //     }
            // }

            if(reverse){
                char [,]arr = new char[b,a];
                // fill the matring by reversing the id
                int k = id.Length-1;
            
                // Fill the matrix column-wise
                for (int j = 0; j < a; j++)
                {
                    for (int i = 0; i < b; i++)
                    {
                        if (k >= 0)
                        {
                            arr[i,j] = s[k];
                        }
                        k--;
                    }
                }
                // Loop to generate
                // encrypted String
                for (int j = 0; j < a; j++)
                {
                    for (int i = 0; i < b; i++)
                    {
                        decrypted = decrypted +
                                    arr[i, j];
                    }
                }


            }
            else{
                char [,]arr = new char[a, b];
                // fill the matring without reversing the id
                int k = 0;
            
                // Fill the matrix column-wise
                for (int j = 0; j < a; j++)
                {
                    for (int i = 0; i < b; i++)
                    {
                        if (k < l)
                        {
                            arr[j, i] = s[k];
                        }
                        k++;
                    }

                    
                }

                // Loop to generate
                // encrypted String
                for (int j = 0; j < b; j++)
                {
                    for (int i = 0; i < a; i++)
                    {
                        decrypted = decrypted +
                                    arr[i, j];
                    }
                }
            }
            decrypted = decrypted+lastChar;
            return decrypted;
        } 

    }
}