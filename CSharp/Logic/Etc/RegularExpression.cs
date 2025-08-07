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
            // 1. 그룹과 캡처 예제
            {
                string input = "Name: Kim, Age: 32 / Name: Lee, Age: 25";
                string pattern = @"Name:\s*(\w+),\s*Age:\s*(\d+)";
                var matches = Regex.Matches(input, pattern);
                foreach (Match m in matches)
                {
                    Console.WriteLine($"전체: {m.Value}, 이름: {m.Groups[1].Value}, 나이: {m.Groups[2].Value}");
                    // 전체: Name: Kim, Age: 32, 이름: Kim, 나이: 32
                    // 전체: Name: Lee, Age: 25, 이름: Lee, 나이: 25
                }
            }

            // 2. 치환(Replace) 예제
            {
                string input = "dog, cat, dog, cat";
                string pattern = "dog";
                string replaced = Regex.Replace(input, pattern, "hamster");
                Console.WriteLine(replaced); // hamster, cat, hamster, cat
            }

            // 3. 경계자(\b) 사용: 단어 단위 찾기
            {
                string input = "the theater is there";
                string pattern = @"\bthe\b";
                var matches = Regex.Matches(input, pattern);
                foreach (Match m in matches)
                    Console.WriteLine($"단어 the 위치: {m.Index}"); // 0
            }

            // 4. Positive Lookahead/Lookbehind
            {
                string input = "apple pie, apple tart, apple juice";
                string pattern = @"apple (?=pie)";
                var matches = Regex.Matches(input, pattern);
                foreach (Match m in matches)
                    Console.WriteLine($"'apple' 다음에 pie 있는 경우만: {m.Value}, pos: {m.Index}");
                // 'apple' 다음에 pie 있는 경우만: apple , pos: 0
            }
            {
                string input = "pre-apple, -apple, apple";
                string pattern = @"(?<=pre-)apple";
                var matches = Regex.Matches(input, pattern);
                foreach (Match m in matches)
                    Console.WriteLine($"pre-가 앞에 붙은 apple만: {m.Value}, pos: {m.Index}");
                // pre-가 앞에 붙은 apple만: apple, pos: 4
            }

            // 5. Multiline 모드와 Anchors(^, $)
            {
                string input = "1st line\n2nd line\n3rd line";
                string pattern = @"^(\d+)";
                var matches = Regex.Matches(input, pattern, RegexOptions.Multiline);
                foreach (Match m in matches)
                    Console.WriteLine($"줄 시작 숫자: {m.Value}"); // 1, 2, 3
            }

            // 6. Greedy vs Lazy(Non-Greedy)
            {
                string input = "<b>Bold1</b> <b>Bold2</b>";
                string greedy = @"<b>.*</b>";    // Greedy
                string lazy = @"<b>.*?</b>";     // Lazy
                Console.WriteLine("Greedy : " + Regex.Match(input, greedy).Value); // <b>Bold1</b> <b>Bold2</b>
                Console.WriteLine("Lazy   : " + Regex.Match(input, lazy).Value);   // <b>Bold1</b>
            }

            // 7. Option 사용: 대소문자 구분 없이 찾기
            {
                string input = "Dog, DOG, dog";
                var matches = Regex.Matches(input, "dog", RegexOptions.IgnoreCase);
                foreach (Match m in matches)
                    Console.WriteLine($"dog(case-insensitive): {m.Value}"); // Dog, DOG, dog
            }

            // 8. 숫자 및 날짜 패턴 검증
            {
                string date = "2024-08-07";
                string datePattern = @"^\d{4}-\d{2}-\d{2}$";
                Console.WriteLine("날짜 검증: " + Regex.IsMatch(date, datePattern)); // True
            }

            // 9. 전화번호 패턴 검증
            {
                string phone = "010-1234-5678";
                string pattern = @"^01[016789]-\d{3,4}-\d{4}$";
                Console.WriteLine("전화번호 검증: " + Regex.IsMatch(phone, pattern)); // True
            }

            // 10. Split 예제
            {
                string csv = "apple,banana,kiwi";
                string[] items = Regex.Split(csv, ",");
                Console.WriteLine(string.Join("|", items)); // apple|banana|kiwi
            }
        }

        public static void Test()
        {
            check_regular_expression_detail();

            check_regular_expression();
        }

    }
}
