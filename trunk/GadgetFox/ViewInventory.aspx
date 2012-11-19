<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="ViewInventory.aspx.cs" Inherits="GadgetFox.ViewInventory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- SkinID="Professional" CellPadding="4" HeaderStyle-BackColor="#444444" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#dddddd"-->

    <asp:GridView ID="gdvMonitorInventory" runat="server" CellPadding="4" GridLines="Vertical" SkinID="Professional" HeaderStyle-BackColor="#444444" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#dddddd" BorderStyle="None"
        BorderWidth="1px" AutoGenerateColumns="False" AllowSorting="True" DataKeyNames="ProductID" DataSourceID="SqlDataSource1" AllowPaging="True">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:TemplateField HeaderText="Image" SortExpression="ImageID">
                <ItemTemplate>
                    <asp:Image ID="imgProduct" runat="server" AlternateText='<%# "Image for " + Eval("ProductID") %>' ImageUrl='<%# getImage(Eval("ImageID").ToString()) %>' Height="80px" Width="80px" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="ProductID" HeaderText="ProductID" SortExpression="ProductID" ReadOnly="True" />
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ReadOnly="True" />
            <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" ReadOnly="True" />
            <asp:TemplateField HeaderText="Price" SortExpression="Price">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" 
                        Text='<%# getPrice(Convert.ToDecimal(Eval("Price")),Convert.ToDecimal(Eval("SalePrice")),Convert.ToBoolean(Eval("Insale"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Quantity" HeaderText="Quantity" SortExpression="Quantity" />
            <asp:BoundField DataField="CategoryName" HeaderText="CategoryName" SortExpression="CategoryName" ReadOnly="True" />
            <asp:BoundField DataField="SubCategoryName" HeaderText="SubCategoryName" SortExpression="SubCategoryName" ReadOnly="True" />
            <asp:BoundField DataField="Color" HeaderText="Color" SortExpression="Color" ReadOnly="True" />
            <asp:BoundField DataField="Weight" HeaderText="Weight" SortExpression="Weight" ReadOnly="True" />
            <asp:BoundField DataField="Threshold" HeaderText="Threshold" SortExpression="Threshold" />
            <asp:CommandField ButtonType="Button" ShowEditButton="True" />
        </Columns>
        <EmptyDataTemplate>
            <asp:Label ID="lblNoData" runat="server" Text="No products in inverntory"></asp:Label>
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
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:myConnectionString %>"
        SelectCommand="SELECT vw_ProductDetails.ProductID, vw_ProductDetails.Name, vw_ProductDetails.Description, vw_ProductDetails.Price, vw_ProductDetails.SalePrice, vw_ProductDetails.InSale, vw_ProductDetails.Quantity, vw_ProductDetails.CategoryID, vw_ProductDetails.SubCategoryID, vw_ProductDetails.ImageID, vw_ProductDetails.Color, vw_ProductDetails.Weight, vw_ProductDetails.CategoryName, vw_ProductDetails.SubCategoryName, Thresholds.Threshold FROM vw_ProductDetails LEFT JOIN Thresholds ON vw_ProductDetails.ProductID = Thresholds.ProductID"
        UpdateCommand="sp_UpdateInventory" UpdateCommandType="StoredProcedure">
        <UpdateParameters>
            <asp:Parameter Name="ProductID" />
            <asp:Parameter Name="Threshold" Type="Int32"  />
            <asp:Parameter Name="Quantity" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
</asp:Content>
