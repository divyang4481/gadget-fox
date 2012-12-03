<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="ViewInventory.aspx.cs" Inherits="GadgetFox.ViewInventory" EnableEventValidation="false"%>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p style="height: 0px; margin-bottom: 50px; font-style: normal; font-size: large; top: auto;">
        Warehouse Inventory
    </p>

    <asp:Button ID="btnExportToExcel" runat="server" OnClick="btnExportToExcel_Click1" Text="Export to Excel" Width="107px" />

    <asp:Button ID="btnExportToWord" runat="server" OnClick="btnExportToWord_Click" Text="Export to Word" Width="102px" />

    <asp:LinkButton style="display: block; color: blue; margin-bottom: 10px;" ID="monitorInventoryButton" runat="server" Text="View low inventory" PostBackUrl="~/MonitorInventory.aspx"> </asp:LinkButton>
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
            <asp:TemplateField HeaderText="Quantity" SortExpression="Quantity">
                <EditItemTemplate>
                    <asp:TextBox ID="txtQuantity" runat="server" Text='<%# Bind("Quantity") %>'></asp:TextBox>
                    <asp:CompareValidator ID="cVQuantity" runat="server" Text="*" ErrorMessage="Quantity" ForeColor="Red" ControlToValidate="txtQuantity" Type="Integer" Operator="GreaterThanEqual" ValueToCompare="0"></asp:CompareValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("Quantity") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CategoryName" HeaderText="CategoryName" SortExpression="CategoryName" ReadOnly="True" />
            <asp:BoundField DataField="SubCategoryName" HeaderText="SubCategoryName" SortExpression="SubCategoryName" ReadOnly="True" />
            <asp:BoundField DataField="Color" HeaderText="Color" SortExpression="Color" ReadOnly="True" />
            <asp:BoundField DataField="Weight" HeaderText="Weight" SortExpression="Weight" ReadOnly="True" />
            <asp:TemplateField HeaderText="Threshold" SortExpression="Threshold">
                <EditItemTemplate>
                    <asp:TextBox ID="txtThreshold" runat="server" Text='<%# Bind("Threshold") %>'></asp:TextBox>
                    <asp:CompareValidator ID="cVThreshold" runat="server" Text="*" ErrorMessage="Threshold" ForeColor="Red" ControlToValidate="txtThreshold" Type="Integer" Operator="GreaterThanEqual" ValueToCompare="0"></asp:CompareValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("Threshold") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
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
            <asp:Parameter Name="Threshold" Type="Int32" />
            <asp:Parameter Name="Quantity" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:ValidationSummary ID="validationSummary" runat="server" HeaderText="Not able to update. Data type error for the field(s):" />
</asp:Content>
