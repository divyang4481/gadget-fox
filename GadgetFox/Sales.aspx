<%@ Page Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="Sales.aspx.cs" Inherits="GadgetFox.Sales" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:Label ID="returnLabel" runat="server" ForeColor="#CC0000"></asp:Label>
    </div>
    <p style="height: 0px; margin-bottom: 20px; font-style: normal; font-size: large; top: auto;">
        Sales
    </p>
    <div>
        <asp:GridView ID="GridView1" runat="server" style="margin-top: 20px;" SkinID="Professional" CellPadding="4" HeaderStyle-BackColor="#444444" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#dddddd" OnRowDataBound="GridView1_RowDataBound"/>
    </div>
</asp:Content>
