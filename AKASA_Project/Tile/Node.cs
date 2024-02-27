using System;

[Serializable]
public struct Node
{
    public int row;
    public int column;

    public Node(in Node copy)
    {
        this.row = copy.row;
        this.column = copy.column;
        this.row = copy.row;
    }

    public Node(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public static Node operator +(Node n1, Node n2) => new Node(n1.row + n2.row, n1.column + n2.column);

    public static Node operator -(Node n1, Node n2) => new Node(n1.row - n2.row, n1.column - n2.column);


    public static bool operator ==(Node n1, Node n2) => (n1.Equals(n2));

    public static bool operator !=(Node n1, Node n2) => !n1.Equals(n2);

    /// <summary>
    /// struct key를 사용하는 Dictionary 최적화 
    /// </summary>
    public override bool Equals(object obj)
    {
        if (obj is Node)
        {
            Node node = (Node)obj;
            return row.Equals(node.row) && column.Equals(node.column);
        }
        return false;
    }

    public override int GetHashCode() =>  column ^ row;

    public override string ToString()
    {
        return string.Format("({0} , {1})", row, column);
    }
}
