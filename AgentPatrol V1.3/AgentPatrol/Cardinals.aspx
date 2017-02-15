<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Cardinals.aspx.cs" Inherits="Cardinals" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content" ContentPlaceHolderID="body" runat="Server">
    <form runat="server">
        <asp:ScriptManager EnablePartialRendering="true" EnablePageMethods="true"
            ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

        <script type="text/javascript" id="WindowSize">
            function getPageSize() {
                document.getElementById('<%=PageWidth.ClientID %>').value = document.all ? document.body.clientWidth : window.innerWidth;
                document.getElementById('<%=PageHeight.ClientID %>').value = document.all ? document.body.clientHeight : window.innerHeight;
                __doPostBack('', '');
            }
        </script>

        <asp:Textbox runat="server" ID="PageWidth" />
        <asp:Textbox runat="server" ID="PageHeight" />
        <br />

        <div class="container">
            <div class="left-half">
                <asp:UpdatePanel runat="server" ID="UDPblockView" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel runat="server" ID="blockViewContainer" CssClass="blockView" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnOneStep" />
                        <asp:AsyncPostBackTrigger ControlID="ButtonNStep" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div class="right-half">
                <asp:UpdatePanel runat="server" ID="UDPgraphView" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel runat="server" ID="graphViewContainer" CssClass="graphView" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnOneStep" />
                        <asp:AsyncPostBackTrigger ControlID="ButtonNStep" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
        <br />
        <br />
        <div class="bottom">
            <asp:Button runat="server" ID="btnOneStep" OnClick="btnOneStep_Click" Text="Run One Step" CssClass="button" />

            <asp:Button runat="server" ID="ButtonNStep" OnClick="btnOneStep_Click" Text="Run N Steps" CssClass="button" />


        </div>
    </form>
</asp:Content>
