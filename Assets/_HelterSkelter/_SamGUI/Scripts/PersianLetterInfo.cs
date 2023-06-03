using UnityEngine;
using System.Collections;

public enum PersianLetterName
{
    unnamed,
    space,
    alef_BaKolah,
    alef,
    be,
    pe,
    te,
    se,
    jim,
    che,
    heJimi,
    khe,
    dal,
    zal,
    re,
    ze,
    zhe,
    sin,
    shin,
    sad,
    zad,
    ta,
    za,
    ein,
    ghein,
    fe,
    ghaf,
    kaf,
    gaf,
    lam,
    mim,
    nun,
    vav,
    he,
    ye,
    la,
    hamzeh,
    vavBaHamzeh,
    n1,
    n2,
    n3,
    n4,
    n5,
    n6,
    n7,
    n8,
    n9,
    n0,
    noghteh,
    virgool,
    parantezRast,
    parantezChap,
    Question,
    Dash,
    Column,
}

public enum EnglishLetterName
{
    unnamed,
    n0,n1,n2,n3,n4,n5,n6,n7,n8,n9,
    A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,
    l_a, l_b, l_c, l_d, l_e, l_f, l_g, l_h, l_i, l_j, l_k, l_l, l_m, l_n, l_o, l_p, l_q, l_r, l_s, l_t, l_u, l_v, l_w, l_x, l_y, l_z,
    leftParenthesis,rightParenthesis,dot,dash,comma,column,space,question,
}
public enum SelectedTextureType
{
    main,
    start,
    mid,
    end,
}

public class Letter
{
    public PersianLetterInfo persianLetterInfo;
    public EnglishLetterInfo englishLetterInfo;
    public SelectedTextureType selectedTextureType;
    public Sprite selectedTexture;
    public float localPos_X = 0;
    public float localPos_Y = 0;
    public float width = 0;
}

public class PersianLetterInfo : MonoBehaviour
{
    public PersianLetterName letterName = PersianLetterName.unnamed;
    public string englishEquiv;
    public Sprite sprite_Main;
    public bool hasStartTexture = true;
    public Sprite sprite_Start;
    public bool hasMidTexture = true;
    public Sprite sprite_Mid;
    public bool hasEndTexture = true;
    public Sprite sprite_End;
    public float beforeDist = 0;
    public float afterDist = 0;
}
