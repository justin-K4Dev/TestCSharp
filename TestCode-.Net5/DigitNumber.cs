using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading.Tasks;


class DigitNumber
{
    // sum_{i=l}^{r} floor((coef * i + add) / div) mod mod
    static long SumFloorRangeMod(
        long coef,
        long add,
        long l,
        long r,
        long div,
        long mod)
    {
        long n = r - l + 1;
        long b = coef * l + add;

        return FloorSumMod(n, div, coef, b, mod);
    }

    // sum_{i=0}^{n-1} floor((a*i + b) / m) mod mod
    static long FloorSumMod(long n, long m, long a, long b, long mod)
    {
        long ans = 0;

        while (true)
        {
            if (a >= m)
            {
                long q = a / m;
                ans = (ans + MulMod(TriMod(n - 1, mod), q % mod, mod)) % mod;
                a %= m;
            }

            if (b >= m)
            {
                long q = b / m;
                ans = (ans + MulMod(n % mod, q % mod, mod)) % mod;
                b %= m;
            }

            BigInteger yMaxBig = (BigInteger)a * n + b;

            if (yMaxBig < m)
                break;

            long yMaxDiv = (long)(yMaxBig / m);
            long yMaxMod = (long)(yMaxBig % m);

            n = yMaxDiv;
            b = yMaxMod;

            long temp = m;
            m = a;
            a = temp;
        }

        return ans % mod;
    }

    // n * (n + 1) / 2 mod mod
    static long TriMod(long n, long mod)
    {
        if (n <= 0)
            return 0;

        long a = n;
        long b = n + 1;

        if ((a & 1) == 0)
            a /= 2;
        else
            b /= 2;

        return MulMod(a % mod, b % mod, mod);
    }

    static long MulMod(long a, long b, long mod)
    {
        return ((a % mod) * (b % mod)) % mod;
    }

    static long[] ReadLongs()
    {
        return Console.ReadLine()!
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToArray();
    }

    public static void Test()
    {
        var nmB = ReadLongs();
        long N = nmB[0], M = nmB[1], B = nmB[2];

        var pqrs = ReadLongs();
        long p = pqrs[0], q = pqrs[1], r = pqrs[2], s = pqrs[3];

        int K = int.Parse(Console.ReadLine()!);

        var sb = new StringBuilder();

        long maxValue = Math.Max(p * N + q, r * M + s);

        for (int query = 0; query < K; query++)
        {
            var arr = ReadLongs();
            long a = arr[0], b = arr[1], c = arr[2], d = arr[3];

            long rowCount = c - a + 1;
            long colCount = d - b + 1;

            var digits = new List<int>();

            for (long place = 1; place <= maxValue;)
            {
                long sumX = SumFloorRangeMod(p, q, a, c, place, B);
                long sumY = SumFloorRangeMod(r, s, b, d, place, B);

                long digit =
                    ((colCount % B) * sumX +
                     (rowCount % B) * sumY) % B;

                digits.Add((int)digit);

                if (place > maxValue / B)
                    break;

                place *= B;
            }

            int last = digits.Count - 1;
            while (last > 0 && digits[last] == 0)
                last--;

            for (int i = last; i >= 0; i--)
                sb.Append(digits[i]);

            sb.AppendLine();
        }

        Console.Write(sb.ToString());
    }
}
