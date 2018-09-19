<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sudoku Validator</title>
    <link href="Default.css" rel="stylesheet" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="description" content="SudoKu Validator" />
</head>
<body>
    <h2>Sudoku Validator</h2>
    <form id="form1" runat="server">        
        <div> 
            <asp:Button ID="ButtonDebugValid" runat="server" Visible="False"  OnClick="ButtonDebugValid_Click" Text="Debug Valid" />
            &nbsp;<asp:Button ID="ButtonDebugInValid" runat="server" Visible="False"  Text="Debug InValid" OnClick="ButtonDebugInValid_Click" />
            &nbsp;<asp:Button ID="ButtonDebugBroken" runat="server" Visible="False"  Text="Debug Broken" OnClick="ButtonDebugBroken_Click" /><br />   
            <asp:Button ID="ButtonDebug" runat="server" Height="20px" OnClick="ButtonDebug_Click" ToolTip="Debug" Width="10px" BackColor="#99CCFF" />
            <asp:TextBox ID="TextBoxInput" runat="server"></asp:TextBox>        
            <asp:Button ID="ButtonSubmit" runat="server" Text="Add Row" title="Debug" OnClick="ButtonSubmit_Click" style="width: 78px" />
            <br />
            <asp:Label ID="LabelMessage" height= "25px" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Table ID="TableSudoKu" runat="server" BorderStyle="Solid" BorderWidth="1px"></asp:Table>        
        </div>
        <asp:Button ID="ButtonValidate" runat="server" Text="Validate" Visible="False" OnClick="ButtonValidate_Click" />
        <asp:Button ID="ButtonDone" runat="server" Text="Done" Visible="False" OnClick="ButtonDone_Click" />
        <asp:HiddenField ID="RowData" runat="server" />
    </form>
</body>
</html>
