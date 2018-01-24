using System.Text;
using System.Collections;

namespace RPGParser
{
    public class RPGParser
    {

        const string s_BRACKET = "[";
        const string s_DELIM = "|";
        const string s_SPACE = " ";
        const char c_BRACKET = '[';
        const char c_CLOSE = ']';

        public string parsed_content;



        public RPGParser(string s)
        {
            this.parsed_content = CorrectPunctuation(Parse(s));
        }


        private string[] TokenizeChoices(string s)
        {
            //print ("Tokenizing : " + s);

            if (!s.Contains(s_BRACKET))
            {
                //print ("no brackets: " + s);
                return s.Split('|');
            }

            ArrayList all_tokens = new ArrayList();

            int closing_bracket_counter = 0;

            //need to figure out the regex for this
            //Regex pipeNotInBrackets = new Regex("*|*");
            StringBuilder token = new StringBuilder();
            foreach (char c in s)
            {

                //collecting all characters while
                //waiting for closing bracket
                //---------------------- NESTED -----------------------
                if (closing_bracket_counter > 0)
                {

                    //nested brackets.
                    if (c == c_BRACKET)
                    {
                        closing_bracket_counter++;
                        token.Append(c);//also save the character in the token
                        continue;
                    }


                    //closing brack and not nest. Save token
                    if (c == ']' && closing_bracket_counter == 1)
                    {
                        //closing bracket has been found
                        //and not in a nested bracket
                        //set this counter back to zero.
                        closing_bracket_counter--;

                        if (token.Length > 0)
                        {
                            token.Append(c);//------------------------------- infinite loop?
                            all_tokens.Add(token.ToString());
                            token = new StringBuilder();//reset StringBuilder for next token
                            continue;
                        }
                    }

                    //seeing a closing bracket while in a nested situation
                    else if (c == ']')
                    {
                        //decriment the counter for next iteration
                        closing_bracket_counter--;
                        token.Append(c);//also save the character in the token
                        continue;
                    }
                    else
                    {
                        //normal character.
                        token.Append(c);
                        continue;
                    }
                }
                //------------------ NESTED ----------------------------


                //---------- NON NESTED; NO BRACKETS SEEN YET ----------

                if (c != ' ' && c != '[' && c != ']' && c != '|')
                {
                    token.Append(c);
                }
                else if (c == '[')
                {
                    //we will now continue within brackets
                    closing_bracket_counter = 1;
                    token.Append(c);//------------------------------- infinite loop?
                }
                else if (c == '|')
                {
                    //if seeing |
                    //and the token has content
                    //then we hit an optional segment so append.
                    if (token.Length > 0)
                    {
                        all_tokens.Add(token.ToString());
                        token = new StringBuilder();
                    }

                    //but if the content in the token is empty,
                    //just continue because this is a |
                    //appearing after a bracket
                    continue;
                }

                else
                {
                    //token is completed; add it to the main list
                    if (token.Length > 0)
                    {
                        all_tokens.Add(token.ToString());
                        token = new StringBuilder();
                    }
                }

            }

            //if token has content left in it then we need to add
            //it as a valid tokey. in situations when you get
            //to the end of the string it was getting left off.
            if (token.Length > 0)
            {
                all_tokens.Add(token.ToString());
                token = new StringBuilder();
            }


            string[] found_tokens = new string[all_tokens.Count];
            for (int i = 0; i < all_tokens.Count; i++)
            {
                found_tokens[i] = all_tokens[i].ToString();

            }

            return found_tokens;
        }

        public static string ParseString(string s)
        {
            return (new RPGParser(s).parsed_content);
        }

        private string Parse(string s)
        {

            string[] tokens = TokenizeChoices(s);
            StringBuilder final_result = new StringBuilder();

            foreach (string token in tokens)
            {

                if (token.StartsWith(s_BRACKET))
                {
                    if (token.Length - 2 < 0) return "bad token. check brackets";

                    string without_brckets = token.Substring(1, token.Length - 2);
                    string pick;

                    string[] special_tokens = SplitAvoidingBrackets(without_brckets);
                    pick = PickRandomly(special_tokens);


                    if (pick.Contains(s_BRACKET))
                    {
                        pick = Parse(pick);
                    }



                    final_result.Append(pick);

                    if (!(final_result[final_result.Length - 1] == ' '))
                        final_result.Append(s_SPACE);
                }
                else
                {

                    final_result.Append(token);
                    if (!(final_result[final_result.Length - 1] == ' '))
                        final_result.Append(s_SPACE);
                }


            }

            return final_result.ToString();
        }




        private string[] SplitAvoidingBrackets(string s)
        {
            string[] picks;
            if (!s.Contains(s_BRACKET))
            {
                picks = s.Split('|');
                return picks;
            }

            ArrayList all_tokens = new ArrayList();
            StringBuilder token = new StringBuilder();
            int closing_bracket_counter = 0;

            for (int i = 0; i < s.Length; i++)
            {

                //not in brackets mode
                if (closing_bracket_counter == 0)
                {

                    if (s[i] != '[' && s[i] != '|')
                    {
                        token.Append(s[i]);
                    }
                    else if (s[i] == '|')
                    {
                        if (token.Length > 0)
                            all_tokens.Add(token);
                        token = new StringBuilder();
                    }
                    else if (s[i] == '[')
                    {
                        closing_bracket_counter++;
                        if (token.Length > 0)
                        {
                            //all_tokens.Add(token); //lemon test
                            //don't start a new token,
                            //but make sure there is a space
                            token.Append(' ');
                        }
                        //token = new StringBuilder();
                        token.Append('[');
                    }

                }//end if for not inside brackets


                //in brackets now
                else
                {
                    if (s[i] == '[')
                    {
                        //in a nested bracket
                        closing_bracket_counter++;
                        token.Append(s[i]);
                    }
                    else if (s[i] == ']')
                    {
                        closing_bracket_counter--;
                        token.Append(s[i]);
                        if (closing_bracket_counter == 0)
                        {
                            //bracket is completely closed. no nests. 


                            //all_tokens.Add(token); ------------------------------ 11-21-16 testing BUG
                            //token = new StringBuilder();------------------------------ 11-21-16

                            //attempting to parse inner brackets immediate
                            //and then keep building token
                            string inner_result = Parse(token.ToString());
                            token = new StringBuilder();
                            token.Append(inner_result);
                            //-----------

                        }
                    }
                    else
                    {
                        //an ignored or regular character
                        token.Append(s[i]);
                    }


                }

            }//end for loop

            //if we've falled out of the loop and anything was in the token,
            //add it to the list now
            if (token.Length > 0)
                all_tokens.Add(token);


            string[] found_tokens = new string[all_tokens.Count];
            for (int i = 0; i < all_tokens.Count; i++)
            {
                found_tokens[i] = all_tokens[i].ToString();

            }

            return found_tokens;
        }



        private string PickRandomly(string s)
        {

            string[] picks = s.Split('|');
            System.Random rand = new System.Random();
            return picks[rand.Next(0, picks.Length)];
        }

        private string PickRandomly(string[] ss)
        {
            System.Random rand = new System.Random();
            return ss[rand.Next(0, ss.Length)];
        }


        private string CorrectPunctuation(string s)
        {
            StringBuilder corrected_string = new StringBuilder();
            char c, c_next;
            for (int i = 0; i < s.Length; i++)
            {
                c = s[i];

                if (i + 1 <= s.Length - 1)
                    c_next = s[i + 1];
                else
                    c_next = ' ';
                if (c == ' ')
                {
                    if (c_next == '.' || c_next == ',' || c_next == '?' || c_next == '!' || c_next == '"')
                    {
                        corrected_string.Append(c_next);
                        i++;
                    }
                    else corrected_string.Append(c);
                }
                else
                {
                    corrected_string.Append(c);
                }

            }

            return corrected_string.ToString();
        }







    }
}
