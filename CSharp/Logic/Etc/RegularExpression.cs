using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace Etc
{
    public class RegularExpression
    {
        static void check_regular_expression()
        {
            /*
                메타문자 의미
                ------------------------
                ^           라인의 처음
                $           라인의 마지막
                \w          문자(영숫자) [a-zA - Z_0 - 9]
                \s          Whitespace(공백, 뉴라인, 탭..)
                \d          숫자
                *           Zero 혹은 그 이상
                +           하나 이상
                ?           Zero 혹은 하나
                .           Newline을 제외한 한 문자
                []          가능한 문자들
                [^ ]        가능하지 않은 문자들
                [- ]        가능 문자 범위
                {n,m}       최소 n개, 최대 m개
                ()          그룹
                |           논리 OR
            */
            {
                {
                    string str = "서울시 강남구 역삼동 강남아파트";

                    // 모든 공백 문자 체크
                    var reg_ex = new Regex($"\\s");
                    Match m = reg_ex.Match(str);
                    while (m.Success)
                    {
                        Debug.WriteLine("{0}:{1}", m.Index, m.Value);
                        m = m.NextMatch();
                    }
                }

                {
                    string str = "1111111222a";

                    {
                        // 모든 영문 체크
                        var reg_ex = new Regex($"[a-zA-Z]");
                        Match m = reg_ex.Match(str);
                        while (m.Success)
                        {
                            Debug.WriteLine("{0}:{1}", m.Index, m.Value);
                            m = m.NextMatch();
                        }
                    }
                    {
                        var mc = Regex.Matches(str, $"[a-zA-Z]");
                        Debug.WriteLine($"Found count : {mc.Count}");
                    }
                }

                {
                    string str = "ㅈ1111111222ㄱ";

                    {
                        // 모든 한글 체크
                        var reg_ex = new Regex($"[ㄱ-ㅎ가-힣]");
                        Match m = reg_ex.Match(str);
                        while (m.Success)
                        {
                            Debug.WriteLine("{0}:{1}", m.Index, m.Value);
                            m = m.NextMatch();
                        }
                    }
                    {
                        {
                            var mc = Regex.Matches(str, $"[ㄱ-ㅎ가-힣]");
                            Debug.WriteLine($"Found count : {mc.Count}");
                        }
                    }
                }

                {
                    string str = "adsfsdfsa";

                    // 특수 문자 체크 : 부정문을 사용하여 체크 한다.
                    var reg_ex = new Regex($"[^a-zA-Z0-9ㄱ-ㅎ가-힣]");
                    Match m = reg_ex.Match(str);
                    while (m.Success)
                    {
                        Debug.WriteLine("{0}:{1}", m.Index, m.Value);
                        m = m.NextMatch();
                    }
                }

                {
                    string[] words = { "sod", "eocd", "qixm", "adio", "soo", "pose", "sop", "ops" };
                    string pattern = @"^(?=.*p)(?=.*o)(?=.*s).*$";  // p, o, s 순서 무관 포함 조건

                    var result = words.Where(word => Regex.IsMatch(word, pattern)).ToList();

                    Console.WriteLine("Matched words:");
                    foreach (var word in result)
                    {
                        Console.WriteLine(word);
                    }
                }
            }
        }


        static void check_regular_expression_detail()
        {

        }

        public static void Test()
        {
            check_regular_expression_detail();

            check_regular_expression();
        }

    }
}
