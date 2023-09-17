using FunctionAppWithRedis.Interfaces;
using System.Text;

namespace FunctionAppWithRedis.Services;

public class PeachService : IPeachService
{
    public PeachService() { }
    public string GetWord(long n)
    {
        char[] ch = { 'P', 'E', 'A', 'C', 'H' };
        long divisor = 5;
        long divident = n;
        StringBuilder sbr = new StringBuilder();
        while (divident > divisor)
        {
            long modulus = divident % divisor;
            if (modulus == 0)
            {
                sbr.Insert(0, ch[ch.Length - 1]);
                divident -= 5;
            }
            else
            {
                sbr.Insert(0, ch[modulus - 1]);
            }
            divident /= divisor;
        }

        if (divident > 0)
        {
            sbr.Insert(0, ch[divident - 1]);
        }

        return sbr.ToString();
    }
}
