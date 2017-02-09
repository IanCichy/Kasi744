<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Cardinals.aspx.cs" Inherits="Cardinals" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content" ContentPlaceHolderID="body" runat="Server">
    <form runat="server">
        <asp:ScriptManager EnablePartialRendering="true"
            ID="ScriptManager" runat="server">
        </asp:ScriptManager>

        <div class="container">
            <div class="left-half">
                <asp:UpdatePanel runat="server" ID="Container" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel runat="server" ID="MapContainer" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnOneStep" />
                        <asp:AsyncPostBackTrigger ControlID="ButtonNStep" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div class="right-half">
                <br />
                <br />
                <br />
                <br />

                <asp:Button runat="server" ID="btnOneStep" OnClick="btnOneStep_Click" Text="Run One Step" CssClass="button" />
                <br />
                <br />
                <asp:Button runat="server" ID="ButtonNStep" OnClick="btnOneStep_Click" Text="Run N Steps" CssClass="button" />

                <asp:Button runat="server" ID="test" OnClick"UploadFile_Click" Text="Upload File" />
            </div>
        </div>

    </form>
</asp:Content>
