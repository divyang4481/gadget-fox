<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="ViewInventory.aspx.cs" Inherits="GadgetFox.ViewInventory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="inventoryGrid">
         <asp:GridView ID="gdvInventory" runat="server" CellPadding="4" ForeColor="Black" GridLines="Vertical" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" AutoGenerateColumns="False" 
            AllowSorting="True" AllowPaging="True" DataKeyNames="ProductID" DataSourceID="SqlDataSource1">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField DataField="ProductID" HeaderText="ProductID" ReadOnly="True" SortExpression="ProductID" />
                <asp:BoundField DataField="Name" ReadOnly="True" HeaderText="Name" SortExpression="Name" />
                <asp:BoundField DataField="Description" ReadOnly="True" HeaderText="Description" />
                <asp:BoundField DataField="Price" HeaderText="Price" ReadOnly="True" SortExpression="Price" />
                <asp:BoundField DataField="SalePrice" HeaderText="SalePrice" ReadOnly="True" SortExpression="SalePrice" />
                <asp:CheckBoxField DataField="InSale" HeaderText="InSale" ReadOnly="True" />
                <asp:BoundField DataField="Quantity" HeaderText="Quantity" SortExpression="Quantity" />
                <asp:BoundField DataField="CategoryID" HeaderText="CategoryID" ReadOnly="True" SortExpression="CategoryID" />
                <asp:BoundField DataField="SubCategoryID" HeaderText="SubCategoryID" ReadOnly="True" SortExpression="SubCategoryID" />
                <asp:BoundField DataField="ImageID" ReadOnly="True"  HeaderText="ImageID" />
                <asp:BoundField DataField="Color" HeaderText="Color" ReadOnly="True"  SortExpression="Color" />
                <asp:BoundField DataField="Weight" HeaderText="Weight" ReadOnly="True" SortExpression="Weight" />
                <asp:CommandField ButtonType="Button" ShowEditButton="True" />
                
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="lblNoData" runat="server" Text="Inventory is empty."></asp:Label>
            </EmptyDataTemplate>
            <FooterStyle BackColor="#CCCC99" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
            <RowStyle BackColor="#F7F7DE" />
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#FBFBF2" />
            <SortedAscendingHeaderStyle BackColor="#848384" />
            <SortedDescendingCellStyle BackColor="#EAEAD3" />
            <SortedDescendingHeaderStyle BackColor="#575357" />
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString='<%$ ConnectionStrings:myConnectionString %>' 
            SelectCommand="SELECT * FROM [Products]" 
            UpdateCommand="UPDATE [GadgetFox].[dbo].[Products] SET Quantity=@Quantity WHERE ProductID=@ProductID">
            <UpdateParameters>
                <asp:Parameter Type="String" name="ProductID"/>
                <asp:Parameter Type="String" name="Quantity"/>
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
