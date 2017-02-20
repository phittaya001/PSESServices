<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PesWeb.App.Forms.Security.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div>
                <asp:Label runat="server" Text="Login Name: "></asp:Label>
                <asp:TextBox runat="server" ID="txtLoginName"></asp:TextBox>
            </div>
            <div>
                <asp:Label runat="server" Text="Password: "></asp:Label>
                <asp:TextBox TextMode="Password" runat="server" ID="txtPassword"></asp:TextBox>
            </div>
            <asp:Button runat="server" ID="btnLogin" Text="Login" UseSubmitBehavior="true" OnClick="btnLogin_Click" />
        </div>
    </form>
</body>
</html>
