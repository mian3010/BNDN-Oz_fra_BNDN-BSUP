<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="about.aspx.cs" Inherits="RentIt.about" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <script src="http://code.jquery.com/jquery-latest.js"></script>
    <script src="js/bootstrap.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="navbar navbar-inverse">
                <div class="navbar-inner">
                    <a class="brand" href="/gspm2013/<%= RentIt.Utility.GetTeamPath(Server.MapPath("~/")) %>">RentIt</a>
                    <ul class="nav">
                        <li><a href="/gspm2013/<%= RentIt.Utility.GetTeamPath(Server.MapPath("~/")) %>">Home</a></li>
                        <li><a href="rent.aspx">Rent</a></li>
                        <li class="active"><a href="#">About Us</a></li>
                    </ul>
                </div>
            </div>
            <div class="hero-unit">
                <h1>RentIt</h1>
                <p>A collaboration between SMU and ITU</p>
                <p>
                    <a class="btn btn-primary btn-large" href="http://wiki.smu.edu.sg/is411/">Learn more
                    </a>
                </p>

            </div>
        </div>
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
    </form>

</body>
</html>
