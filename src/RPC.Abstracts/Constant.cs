using System.Text;

namespace RPC.Abstracts; 

public struct Constants {
    public static readonly byte[] CRLF = Encoding.ASCII.GetBytes("\r\n");
    public static readonly byte[] Separator = Encoding.ASCII.GetBytes("&");
}