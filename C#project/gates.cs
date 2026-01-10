public class Main
{
    static void Program
    {
        
    }
    public bool Nand(bool a, bool b) => !(a && b);
    public bool Not(bool a) => Nand(a, a);
    public bool And(bool a, bool b) => Not(Nand(a, b));
    public bool Or(bool a,bool b) => Nand(Not(a),Not(b));
    public bool Xor(bool a,bool b)
    {
        bool k = Nand(a ,b);
        return Nand(Nand(a ,k),Nand(b,k));
    }
    public bool Mux(bool a,bool b,bool sel) => Nand(Nand(a , Not(sel)),Nand(b,sel));
    public bool DMux(bool c,bool d)
    {
        bool a = Not(Nand(c , Not(d)));
        bool b = Not(Nand(c , d));
        return (a,b);
    }
    public bool Not16(bool[] k)
    {
        bool[] b = new bool[16];
        for(int i = 0;i < 16;i++)
        {
            b[i] = Not(k[i]);
        }
        return b;
    }
    public bool And16(bool[] A,bool[] B)
    {
        bool[] b = new bool[16];
        for(int i = 0;i < 16;i++)
        {
            b[i] = And(A[i],B[i]);
        }
        return b;
    }
    public bool Or16(bool[] A,bool[] B)
    {
        bool[] b = new bool[16];
        for(int i = 0;i < 16;i++)
        {
            b[i] = Or(A[i],B[i]);
        }
        return b;
    }
    public bool Mux16(bool[] A,bool[] B,bool[] Sel)
    {
        bool[] b = new bool[16];
        bool notsel = Not(Sel);
        for(int i = 0;i < 16;i++)
        {
            b[i] = Nand(Nand(A[i] , notsel),Nand(B[i],Sel));
        }
        return b;
    }
    public bool Or8Way(bool[8] a)
    {
        bool k = Or(a[0],a[1]);
        for(int i = 2;i < 8;i++)
        {
            k = Or(k,a[i]);
        }
        return k;
    }
    public bool Mux4Way16(bool[16] a,bool[16] b,bool[16] c,bool[16] d,bool[2] sel)
    {
        bool[] ab = new bool[16];
        bool[] cd = new bool[16];
        bool[] abcd = new bool[16];
        bool notsel = Not(sel[0]);
        for(int i = 0;i < 16;i++)
        {
            ab[i] = Nand(Nand(a[i] , notsel),Nand(b[i],sel[0]));
            cd[i] = Nand(Nand(a[i] , notsel),Nand(b[i],sel[0]));
        }
        abcd = Mux16(ab,cd,sel[1]);
        return abcd;
    }
    public bool Mux8Way16(bool[16] a,bool[16] b,bool[16] c,bool[16] d,bool[16] e,bool[16] f,bool[16] g,bool[16] h,bool[3] sel)
    {
        bool[] ab = new bool[16];
        bool[] cd = new bool[16];
        bool[] ef = new bool[16];
        bool[] gh = new bool[16];
        bool[] abcd = new bool[16];
        bool[] efgh = new bool[16];
        bool[] abcdefgh = new bool[16];
        bool notsel0 = Not(sel[0]);
        bool notsel1 = Not(sel[1]);
        for(int i = 0;i < 16;i++)
        {
            ab[i] = Nand(Nand(a[i] , notsel0),Nand(b[i],sel[0]));
            cd[i] = Nand(Nand(c[i] , notsel0),Nand(d[i],sel[0]));
            ef[i] = Nand(Nand(e[i] , notsel0),Nand(f[i],sel[0]));
            gh[i] = Nand(Nand(g[i] , notsel0),Nand(h[i],sel[0]));
        }
        for(int i = 0;i < 16;i++)
        {
            abcd[i] = Nand(Nand(ab[i] , notsel1),Nand(cd[i],sel[1]));
            efgh[i] = Nand(Nand(ef[i] , notsel1),Nand(gh[i],sel[1]));
        }
        abcdefgh = Mux16(abcd,efgh,sel[2]);
        return abcdefgh;
    }
    public bool DMux4Way(bool e,bool[2] sel)
    {
        bool Notsel0 = Not(sel[0]);
        bool ab = Not(Nand(e , Not(sel[1])));
        bool cd = Not(Nand(e , sel[1]));
        bool a = Not(Nand(ab , Notsel0));
        bool b = Not(Nand(ab , sel[0]));
        bool c = Not(Nand(cd , Notsel0));
        bool d = Not(Nand(cd , sel[0]));
        return (bool a,bool b,bool c,bool d);
    }
    public bool DMux8Way(bool j,bool[3] sel)
    {
        bool[] k = new bool[2];
        k[1] = sel[2];
        k[0] = sel[1];
        var (ab , cd , ef , gh) = Mux4Way16(j,k);
        bool Notsel0 = Not(sel[0]);
        bool a = Not(Nand(ab , Notsel0));
        bool b = Not(Nand(ab , sel[0]));
        bool c = Not(Nand(cd , Notsel0));
        bool d = Not(Nand(cd , sel[0]));
        bool e = Not(Nand(ef , Notsel0));
        bool f = Not(Nand(ef , sel[0]));
        bool g = Not(Nand(gh , Notsel0));
        bool h = Not(Nand(gh , sel[0]));
        return (bool a,bool b,bool c,bool d,bool e,bool f,bool g,bool h);
    }
    public bool HalfAdder(bool a,bool b)
    {
        bool k = Nand(a , b);
        bool carry = Not(k);
        bool sum = Nand(Nand(a,k),Nand(b,k));
        return (carry ,sum);
    }
    public bool Adder(bool a,bool b,bool c)
    {
        bool k = Nand(a,b);
        bool d = Nand(Nand(a,k),Nand(b,k));
        bool l = Nand(c,d);
        bool sum = Nand(Nand(c,l),Nand(d,l));
        bool carry = Nand(k,l);
        return (carry,sum);
    }
    public bool Adder16(bool[16] a,bool[16] b)
    {
        bool[] result = new bool[16];
        bool carry;
        (carry,result[0]) = HalfAdder(a[0],b[0]);
        for(int i = 1;i < 16;i++)
        {
            (carry,result[i]) = Adder(a[i],b[i],carry);
        }
        return result;
    }
    public bool Inc16(bool[16] a)
    {
        bool[] result = new bool[16];
        bool carry = a[0];
        result[0] = Not(a[0]);
        for(int i = 1;i < 16;i++)
        {
            (carry,result[i]) = HalfAdder(a[i],carry);
        }
        return result;
    }
    public bool AndAdder16(bool[16] a,bool[16] b)
    {
        bool[] result = new bool[16];
        bool[] carry = new bool[16];
        (carry[0],result[0]) = HalfAdder(a[0],b[0]);
        for(int i = 1;i < 16;i++)
        {
            (carry[i],result[i]) = Adder(a[i],b[i],carry[i-1]);
        }
        return (carry,result);
    }

    public bool M(bool[16] a,bool b)
    {
        bool[] result = new bool[16];
        bool notb = Not(b);
        for(int i = 0;i < 16;i++)
        {
            result[i] = And(notb,a[i]);
        }
        return result;
    }
    public bool Nm(bool[16] a,bool b)
    {
        bool[] result = new bool[16];
        for(int i = 0;i < 16;i++)
        {
            result[i] = Xor(a[i],b);
        }
        return result;
    }
    public bool ALU(bool[16] x,bool[16] y,bool zx,bool nx,bool zy,bool ny,bool f,bool no)
    {
        x = M(x,zx);
        x = Nm(x,nx);
        y = M(y,zy);
        y = Nm(y,ny);
        (x , y) = AndAdder16(x ,y);
        x = Mux16(x,y,f);
        x = Nm(x ,no);
        bool[] result = x;
        bool ng = x[15];
        bool[] Low8 = Or8Way(x[0..7]);
        bool[] high8 = Or8Way(x[8..15]);
        bool zr = Not(Or(high8,Low8));
        return(result,zr,ng);
    }
    
}
public class DFF
{
    private bool save;
    public bool Compute(bool input,bool tick)
    {
        if(tick) save = input;
        return save;
    }
}