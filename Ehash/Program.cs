using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Ehash
{
 class Program
 {
  static void Main(string[] args)
  {

   Console.ForegroundColor = ConsoleColor.Green;

   new EHashStress().MakeTest();

   EHash h = new EHash();
   ulong s = h.Compute(Encoding.ASCII.GetBytes("Eddy^CZ"));

   if (s == 18120688786459961260)
   {
    Console.WriteLine("OK");
   }

   while (true)
   {
    Console.WriteLine("Type what you wanna hash:\n");
    Console.WriteLine(String.Format("\nResult: {0}", h.Compute(Encoding.ASCII.GetBytes(Console.ReadLine()))));
    Console.ReadKey();
   }
  }
 }


 class EHashStress
 {
  public void MakeTest()
  {

   EHash h = new EHash();

   List<ulong> hashs = new List<ulong>();

   byte[] buff = new byte[1024];

   int counter = 0;
   for (;;)
   {
    Console.WriteLine("Test : " + counter);
    new RNGCryptoServiceProvider(counter.ToString()).GetBytes(buff);
    ulong a = h.Compute(buff);
    if (!hashs.Contains(a))
    {
     hashs.Add(a);
     Console.Write(" = Unique hash ;)\n");
    }
    else
    {
     Console.Write("= Error hash same! ;)\n");
     Console.ReadKey();
    }
    counter++;
   }

  }
 }



 public class EHash
 {

  private const int hash_lengt = 26;

  private ulong MakeNumber(char str)
  {
   string s = Make8BitString(str);
   ulong u = ToUlong(s);
   return u;
  }
  private ulong[] Generate(string s)
  {
   ulong[] ul = new ulong[s.Length];
   for (int i = 0; i < ul.Length; i++)
   {
    ul[i] = Shuffle(MakeNumber(s[i]));
   }
   return ul;
  }

  private string Transform(char ch)
  {
   return Encoding.ASCII.GetString(ToByteArr(Generate(Convert.ToBase64String(BitConverter.GetBytes(ch)))));
  }

  public string PreMake(string str)
  {
   string s = String.Empty;
   int t = (int)str[0];
   int n = (int)str[str.Length - 1];
   foreach (char ch in str.ToCharArray())
   {
    t <<= str.Length;
    int w = (int)(ch ^ t);
    int o = (int)ch >> w & (t) | n;
    s += Transform((char)w).ToUpper();
    s += ToHex(o);
    t++;
    n++;
   }
   return s.Replace("F", "").Replace("0", "");
  }

  private BigInteger Multiply(BigInteger number)
  {
   return BigInteger.Multiply(number, hash_lengt / 2);
  }

  private ulong Checksum(string s)
  {
   List<ulong> ul = new List<ulong>();
   for (int i = 0; i < s.Length / 8; i++)
   {
    string pp = String.Empty;
    int couter = 0;
    while (couter != 8)
    {
     pp += s[i + couter];
     couter++;
    }
    ul.Add(ulong.Parse(pp));
   }
   ulong key0 = 0;
   ulong key1 = 0;
   foreach (ulong u in ul)
   {
    key1 ^= u;
    key0 -= u;

   }
   return key1 * key0;
  }

  public ulong Compute(byte[] data)
  {
   string str = String.Empty;
   byte[] pre = Encoding.ASCII.GetBytes(PreMake(Convert.ToBase64String(SHA512.Create().ComputeHash(data))));
   ulong p = Checksum(Multiply(new BigInteger(pre)).ToString());
   return p;
  }

  private unsafe string ToHex(int n)
  {
   int* i = (int*)IntPtr.Zero;
   int nt = 8;
   while (nt != 0)
   {
    i -= (int*)n;
    nt--;
   }
   int* t = (int*)(IntPtr.Add((IntPtr)i, n));
   IntPtr p = (IntPtr)((int)(IntPtr)(i) ^ (int)Math.Exp((double)(int)(IntPtr)(t)));
   return p.ToString("X");
  }
  private string Make8BitString(char str)
  {
   byte[] ch = SHA1.Create().ComputeHash(BitConverter.GetBytes(str));
   var s = String.Empty;
   var rnd = new Random();

   for (int i = 0; i < 4; i++)
   {
    s += ch[i].ToString("x2");
   }
   return s;
  }

  private unsafe ulong ToUlong(string str)
  {
   return BitConverter.ToUInt64((Encoding.ASCII.GetBytes(str)), 0);
  }
  public byte[] ToByteArr(ulong[] arr)
  {
   return arr.Select(u => (byte)(u | u << 8 | u << 16 | u << 24)).ToArray();
  }

  private ulong Shuffle(ulong num)
  {
   byte[] b = BitConverter.GetBytes(num);
   int pos = 0;
   while (pos != b.Length - 1)
   {
    b[pos++] = b[pos];
   }
   return BitConverter.ToUInt64(b, 0);
  }
 }
}
