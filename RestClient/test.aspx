<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="RestClient.test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="Auth" runat="server" OnClick="Auth_Click" Text="Test auth" /><br />
            <asp:Label ID="AuthResponseLabel" runat="server" Text="Auth response: "></asp:Label><asp:Label ID="AuthResponse" runat="server"></asp:Label><br />
            <asp:Button ID="GetAccount" runat="server" OnClick="GetAccount_Click" Text="Test GetAccount" /><br />
            <asp:Label ID="GetAccountResponseLabel" runat="server" Text="GetAccount response: "></asp:Label><asp:Label ID="GetAccountResponse" runat="server"></asp:Label><br />
            <asp:Label ID="TokeLabel" runat="server" Text="Token: "></asp:Label><asp:TextBox runat="server" ID="Token"></asp:TextBox><br />
            <asp:Button ID="Update" runat="server" OnClick="Update_Click" Text="Test update (REMEMBER TOKEN)" /><br />
            <asp:Label ID="UpdateResponseLabel" runat="server" Text="Update response: "></asp:Label><asp:Label ID="UpdateResponse" runat="server"></asp:Label><br />
            <asp:Button ID="Create" runat="server" OnClick="Create_Click" Text="Test create" /><br />
            <asp:Label ID="CreateLabel" runat="server" Text="Create response: "></asp:Label><asp:Label ID="CreateResponse" runat="server"></asp:Label><br />
            <asp:Button ID="GetAccounts" runat="server" OnClick="GetAccounts_Click" Text="Test GetAccounts" /><br />
            <asp:Label ID="GetAccountsResponseLabel" runat="server" Text="GetAccounts response: "></asp:Label><asp:Label ID="GetAccountsResponse" runat="server"></asp:Label><br />
        </div>
    </form>
</body>
</html>
