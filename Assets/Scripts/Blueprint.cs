public class Blueprint
{
    public string itemName;
    public int numOfRequirements;
    public string Req1;
    public int Reg1amount;
    public string Req2;
    public int Reg2amount;

    public Blueprint(string name, int reqNUM, string R1, int R1num, string R2, int R2num)
    {
        itemName = name;
        numOfRequirements = reqNUM;
        Req1 = R1;
        Reg1amount = R1num; 
        Req2 = R2;
        Reg2amount = R2num;
    }
}