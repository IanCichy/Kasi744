<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Cardinals.aspx.cs" Inherits="Cardinals" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

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
        <asp:HiddenField runat="server" ID="PageWidth" />
        <asp:HiddenField runat="server" ID="PageHeight" />
        <br />

        <div class="container">
            <asp:UpdatePanel runat="server" ID="UDPContained" UpdateMode="Always">
                <ContentTemplate>



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

                    <!--THIS IS THE POPUP FOR THE NODE CLICK-->
                    <asp:Button ID="btnFake" runat="server" Style="display: none;" />
                    <ajax:ModalPopupExtender
                        ID="mpe"
                        runat="server"
                        PopupControlID="pnlPopup"
                        TargetControlID="btnFake"
                        OkControlID="btnClose"
                        BackgroundCssClass="modalBackground"
                        PopupDragHandleControlID="pnlPopup" />
                    <asp:Panel
                        ID="pnlPopup"
                        runat="server"
                        CssClass="modalPopup">
                        <div class="popupHeader">
                            Agent Information
                        </div>
                        <div class="body">
                            <b>INFO:</b>
                            <asp:Label ID="lblInfo" runat="server" Text="PLACEHOLDER" /><br />
                        </div>
                        <div class="footer">
                            <asp:Button ID="btnClose" runat="server" Text="Okay" />
                        </div>
                    </asp:Panel>
                    <!--THIS IS THE POPUP FOR THE NODE CLICK-->
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <br />
        <br />
        <div class="bottom">
            <asp:Button runat="server" ID="btnOneStep" OnClick="btnOneStep_Click" Text="Run One Step" CssClass="button" />
            <asp:Button runat="server" ID="ButtonNStep" OnClick="btnOneStep_Click" Text="Run N Steps" CssClass="button" />
            <p>
                <asp:FileUpload ID="file" runat="server" CssClass="button" />
                <asp:Button runat="server" ID="uploadMap" OnClick="uploadMap_Click" Text="Upload Map" CssClass="button" />
                <asp:Label ID="Label1" runat="server" Text="" Style="color: Red"></asp:Label>
            </p>
        </div>
    </form>
</asp:Content>
