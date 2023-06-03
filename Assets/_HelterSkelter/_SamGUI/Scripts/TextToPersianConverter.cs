using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TextToPersianConverter : MonoBehaviour
{

    public PersianLetterInfo[] persianLetterInfos;
    public EnglishLetterInfo[] englishLetterInfos;

    public List<Letter> ConvertToPersian(string _text)
    {
        List<Letter> result = new List<Letter>();

        if (_text.Length == 0)
            return result;

        char[] datTextChars = _text.ToCharArray(0, _text.Length);

        string[] datTextEnglishLetters = new string[datTextChars.Length];
        for (int i = 0; i < datTextEnglishLetters.Length; i++)
        {
            datTextEnglishLetters[i] = datTextChars[i].ToString();
        }

        int curindex = 0;

        while (curindex < datTextEnglishLetters.Length)
        {
            Letter newSelectedLet = new Letter();

            string curEnglishLetter = datTextEnglishLetters[curindex];
            string nextEnglishLetter = "";

            PersianLetterInfo curLetterPersianInfo = FindPersianLetterByEnglishEquiv(curEnglishLetter);
            PersianLetterInfo beforeLetterPersianInfo = null;
            PersianLetterInfo afterLetterPersianInfo = null;

            string beforeEnglishLetter = "";
            bool hasALetterBefore = false;
            string afterEnglishLetter = "";
            bool hasALetterAfter = false;

            if (curindex > 0)
            {
                hasALetterBefore = true;
                beforeEnglishLetter = datTextEnglishLetters[curindex - 1];
                beforeLetterPersianInfo = FindPersianLetterByEnglishEquiv(beforeEnglishLetter);
            }

            if (curindex < datTextChars.Length - 1)
            {
                nextEnglishLetter = datTextEnglishLetters[curindex + 1];

                hasALetterAfter = true;
                afterEnglishLetter = datTextEnglishLetters[curindex + 1];
                afterLetterPersianInfo = FindPersianLetterByEnglishEquiv(afterEnglishLetter);
            }

            newSelectedLet.persianLetterInfo = curLetterPersianInfo;

            if (IsCurrentEnglishLetterANumber(curEnglishLetter) && hasALetterAfter && IsCurrentEnglishLetterANumber(nextEnglishLetter))
            {
                #region 2 or more numbers
                List<string> nums = new List<string>();
                int ind = curindex;

                for (ind = curindex; ind < datTextEnglishLetters.Length; ind++)
                {
                    if (IsCurrentEnglishLetterANumber(datTextEnglishLetters[ind]))
                        nums.Add(datTextEnglishLetters[ind]);
                    else
                        break;
                }

                for (int numInd = nums.Count - 1; numInd >= 0; numInd--)
                {
                    Letter selLet = new Letter();
                    selLet.persianLetterInfo = FindPersianLetterByEnglishEquiv(nums[numInd]);
                    selLet.selectedTextureType = SelectedTextureType.main;
                    selLet.selectedTexture = selLet.persianLetterInfo.sprite_Main;
                    result.Add(selLet);
                }

                curindex = ind;
                continue;
                #endregion
            }
            else
            {

                if (hasALetterBefore)
                {
                    #region hasALetterAfter and else
                    if (hasALetterAfter)
                    {
                        if (beforeLetterPersianInfo.hasStartTexture)
                        {
                            #region afterLetterPersianInfo.hasEndTexture or else
                            if (afterLetterPersianInfo.hasEndTexture)
                            {
                                if (curLetterPersianInfo.hasMidTexture)
                                {
                                    newSelectedLet.selectedTextureType = SelectedTextureType.mid;
                                    newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_Mid;
                                    result.Add(newSelectedLet);
                                    curindex++;
                                    continue;
                                }
                                else
                                {
                                    if (curLetterPersianInfo.hasEndTexture)
                                    {
                                        newSelectedLet.selectedTextureType = SelectedTextureType.end;
                                        newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_End;
                                        result.Add(newSelectedLet);
                                        curindex++;
                                        continue;
                                    }
                                    else
                                    {
                                        newSelectedLet.selectedTextureType = SelectedTextureType.main;
                                        newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_Main;
                                        result.Add(newSelectedLet);
                                        curindex++;
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                if (curLetterPersianInfo.hasEndTexture)
                                {
                                    newSelectedLet.selectedTextureType = SelectedTextureType.end;
                                    newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_End;
                                    result.Add(newSelectedLet);
                                    curindex++;
                                    continue;
                                }
                                else
                                {
                                    newSelectedLet.selectedTextureType = SelectedTextureType.main;
                                    newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_Main;
                                    result.Add(newSelectedLet);
                                    curindex++;
                                    continue;
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region afterLetterPersianInfo.hasEndTexture or else
                            if (afterLetterPersianInfo.hasEndTexture)
                            {
                                if (curLetterPersianInfo.hasStartTexture)
                                {
                                    newSelectedLet.selectedTextureType = SelectedTextureType.start;
                                    newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_Start;
                                    result.Add(newSelectedLet);
                                    curindex++;
                                    continue;
                                }
                                else
                                {
                                    newSelectedLet.selectedTextureType = SelectedTextureType.main;
                                    newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_Main;
                                    result.Add(newSelectedLet);
                                    curindex++;
                                    continue;
                                }
                            }
                            else
                            {
                                newSelectedLet.selectedTextureType = SelectedTextureType.main;
                                newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_Main;
                                result.Add(newSelectedLet);
                                curindex++;
                                continue;
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        if (beforeLetterPersianInfo.hasStartTexture)
                        {

                            #region curLetterPersianInfo.hasEndTexture or else
                            if (curLetterPersianInfo.hasEndTexture)
                            {
                                newSelectedLet.selectedTextureType = SelectedTextureType.end;
                                newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_End;
                                result.Add(newSelectedLet);
                                curindex++;
                                continue;
                            }
                            else
                            {
                                newSelectedLet.selectedTextureType = SelectedTextureType.main;
                                newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_Main;
                                result.Add(newSelectedLet);
                                curindex++;
                                continue;
                            }
                            #endregion

                        }
                        else
                        {
                            #region just main

                            newSelectedLet.selectedTextureType = SelectedTextureType.main;
                            newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_Main;
                            result.Add(newSelectedLet);
                            curindex++;
                            continue;

                            #endregion
                        }
                    }
                    #endregion
                }
                else
                {
                    #region hasALetterAfter and else
                    if (hasALetterAfter)
                    {

                        #region afterLetterPersianInfo.hasEndTexture or else
                        if (afterLetterPersianInfo.hasEndTexture)
                        {
                            if (curLetterPersianInfo.hasStartTexture)
                            {
                                newSelectedLet.selectedTextureType = SelectedTextureType.start;
                                newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_Start;
                                result.Add(newSelectedLet);
                                curindex++;
                                continue;
                            }
                            else
                            {
                                newSelectedLet.selectedTextureType = SelectedTextureType.main;
                                newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_Main;
                                result.Add(newSelectedLet);
                                curindex++;
                                continue;
                            }
                        }
                        else
                        {
                            newSelectedLet.selectedTextureType = SelectedTextureType.main;
                            newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_Main;
                            result.Add(newSelectedLet);
                            curindex++;
                            continue;
                        }
                        #endregion

                    }
                    else
                    {
                        #region just main

                        newSelectedLet.selectedTextureType = SelectedTextureType.main;
                        newSelectedLet.selectedTexture = newSelectedLet.persianLetterInfo.sprite_Main;
                        result.Add(newSelectedLet);
                        curindex++;
                        continue;

                        #endregion
                    }
                    #endregion
                }
            }
        }

        return result;
    }

    public List<Letter> GetEnglishLetters(string _text)
    {
        List<Letter> result = new List<Letter>();

        if (_text.Length == 0)
            return result;

        char[] datTextChars = _text.ToCharArray(0, _text.Length);

        string[] datTextEnglishLetters = new string[datTextChars.Length];
        for (int i = 0; i < datTextEnglishLetters.Length; i++)
        {
            datTextEnglishLetters[i] = datTextChars[i].ToString();
        }

        int curindex = 0;

        while (curindex < datTextEnglishLetters.Length)
        {
            Letter newSelectedLet = new Letter();

            string curEnglishLetter = datTextEnglishLetters[curindex];

            EnglishLetterInfo curLetterInfo = FindEnglishLetterInfo(curEnglishLetter);

            newSelectedLet.englishLetterInfo = curLetterInfo;

            newSelectedLet.selectedTextureType = SelectedTextureType.main;
            newSelectedLet.selectedTexture = newSelectedLet.englishLetterInfo.sprite;
            result.Add(newSelectedLet);
            curindex++;
            continue;
        }

        return result;
    }

    public PersianLetterInfo FindPersianLetterByName(PersianLetterName _letterName)
    {
        for (int i = 0; i < persianLetterInfos.Length; i++)
        {
            if (persianLetterInfos[i].letterName == _letterName)
                return persianLetterInfos[i];
        }

        return null;
    }

    PersianLetterInfo FindPersianLetterByEnglishEquiv(string _englishEquiv)
    {
        for (int i = 0; i < persianLetterInfos.Length; i++)
        {
            if (persianLetterInfos[i].englishEquiv == _englishEquiv)
                return persianLetterInfos[i];
        }

        return null;
    }

    EnglishLetterInfo FindEnglishLetterInfo(string _char)
    {
        for (int i = 0; i < englishLetterInfos.Length; i++)
        {
            if (englishLetterInfos[i].character == _char)
                return englishLetterInfos[i];
        }

        return null;
    }

    public bool IsCurrentEnglishLetterANumber(string _engLetter)
    {
        string engLetter = _engLetter;

        if (engLetter == "0" || engLetter == "1" || engLetter == "2" || engLetter == "3" || engLetter == "4"
           || engLetter == "5" || engLetter == "6" || engLetter == "7" || engLetter == "8" || engLetter == "9")
            return true;

        return false;
    }

    public string GetEnglishLetterByKeyboardKeyName(string _keyName)
    {
        string keyName = _keyName;

        switch (keyName)
        {
            case "Key_Zad":
                return "q";

            case "Key_Sad":
                return "w";

            case "Key_Se":
                return "e";

            case "Key_Ghaf":
                return "r";

            case "Key_Fe":
                return "t";

            case "Key_Ghein":
                return "y";

            case "Key_Ein":
                return "u";

            case "Key_He":
                return "i";

            case "Key_Khe":
                return "o";

            case "Key_HeJimmy":
                return "p";

            case "Key_Jim":
                return "[";

            case "Key_Che":
                return "]";

            case "Key_Shin":
                return "a";

            case "Key_Sin":
                return "s";

            case "Key_Ye":
                return "d";

            case "Key_Be":
                return "f";

            case "Key_Lam":
                return "g";

            case "Key_Alef":
                return "h";

            case "Key_AlefBaKolah":
                return "H";

            case "Key_Te":
                return "j";

            case "Key_Nun":
                return "k";

            case "Key_Mim":
                return "l";

            case "Key_Kaf":
                return ";";

            case "Key_Gaf":
                return "'";

            case "Key_Za":
                return "z";

            case "Key_Ta":
                return "x";

            case "Key_Zhe":
                return "C";

            case "Key_Ze":
                return "c";

            case "Key_Re":
                return "v";

            case "Key_Zal":
                return "b";

            case "Key_Dal":
                return "n";

            case "Key_Hamzeh":
                return "m";

            case "Key_Vav":
                return ",";

            case "Key_Pe":
                return "/";

            case "Key_Space":
                return " ";

            case "Key_Noghteh":
                return ".";

            case "Key_Virgool":
                return "T";

            case "Key_ParantezRast":
                return ")";

            case "Key_ParantezChap":
                return "(";

            case "Key_1":
                return "1";

            case "Key_2":
                return "2";

            case "Key_3":
                return "3";

            case "Key_4":
                return "4";

            case "Key_5":
                return "5";

            case "Key_6":
                return "6";

            case "Key_7":
                return "7";

            case "Key_8":
                return "8";

            case "Key_9":
                return "9";

            case "Key_0":
                return "0";
        }

        return "hajfhi";
    }
}
