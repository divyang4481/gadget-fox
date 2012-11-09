<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Image.aspx.cs" Inherits="GadgetFox.Image" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString='<%$ ConnectionStrings:myConnectionString %>'
                SelectCommand="SELECT * FROM [viewCart]"
                UpdateCommand="UPDATE [GadgetFox].[dbo].[Carts] SET Quantity=@Quantity WHERE ProductID=@ProductID and ">
                <UpdateParameters>
                    <asp:Parameter Type="String" Name="ProductID" />
                    <asp:Parameter Type="String" Name="Quantity" />
                </UpdateParameters>
            </asp:SqlDataSource>
        </div>
    </form>
</body>
</html>
