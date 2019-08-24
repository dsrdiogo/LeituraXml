Imports System.Xml
Imports System.IO

Public Class Form1

    Private Sub Frm_ImportaXmlCte_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        Call ConfiguraGrid()

    End Sub

    Private Sub ConfiguraGrid()


        With grdNota

            .RowCount = 0
            .ColumnCount = 0

            .Columns.Add(New DataGridViewTextBoxColumn())
            .Columns.Add(New DataGridViewTextBoxColumn())
            .Columns.Add(New DataGridViewTextBoxColumn())
            .Columns.Add(New DataGridViewTextBoxColumn())
            .Columns.Add(New DataGridViewTextBoxColumn())
            .Columns.Add(New DataGridViewTextBoxColumn())
            .Columns.Add(New DataGridViewTextBoxColumn())
            .Columns.Add(New DataGridViewTextBoxColumn())
            .Columns.Add(New DataGridViewTextBoxColumn())
            .Columns.Add(New DataGridViewTextBoxColumn())
            .Columns.Add(New DataGridViewTextBoxColumn())
            .Columns.Add(New DataGridViewTextBoxColumn())
            .Columns.Add(New DataGridViewTextBoxColumn())
            .Columns.Add(New DataGridViewTextBoxColumn())


            .Columns(0).Name = "idCte"
            .Columns(1).Name = "numeroCte"
            .Columns(2).Name = "chaveAcessoCte"
            .Columns(3).Name = "dataEmissaoCte"
            .Columns(4).Name = "valorCte"
            .Columns(5).Name = "idNfe"
            .Columns(6).Name = "numeroNfe"
            .Columns(7).Name = "chaveAcessoNfe"
            .Columns(8).Name = "idCteAtual"
            .Columns(9).Name = "numeroCteAtual"
            .Columns(10).Name = "chaveAcessoCteAtual"
            .Columns(11).Name = "valorCteAtual"
            .Columns(12).Name = "emitente"
            .Columns(13).Name = "codFatura"



            .Columns("idCte").Name = "IdCte"
            .Columns("numeroCte").Name = "numeroCte"
            .Columns("chaveAcessoCte").Name = "chaveAcessoCte"
            .Columns("dataEmissaoCte").Name = "dataEmissaoCte"
            .Columns("idNfe").Name = "IdNfe"
            .Columns("numeroNfe").Name = "numeroNfe"
            .Columns("chaveAcessoNfe").Name = "chaveAcessoNfe"
            .Columns("idCteAtual").Name = "idCteAtual"
            .Columns("numeroCteAtual").Name = "numeroCteAtual"
            .Columns("chaveAcessoCteAtual").Name = "chaveAcessoCteAtual"
            .Columns("emitente").Name = "emitente"
            .Columns("codFatura").Name = "codFatura"
            .Columns("valorCte").Name = "valorCte"
            .Columns("valorCteAtual").Name = "ValorCteAtual"


            .Columns("idCte").ReadOnly = True
            .Columns("numeroCte").ReadOnly = True
            .Columns("chaveAcessoCte").ReadOnly = True
            .Columns("dataEmissaoCte").ReadOnly = True
            .Columns("idNfe").ReadOnly = True
            .Columns("numeroNfe").ReadOnly = True
            .Columns("chaveAcessoNfe").ReadOnly = True
            .Columns("idCteAtual").ReadOnly = True
            .Columns("numeroCteAtual").ReadOnly = True
            .Columns("chaveAcessoCteAtual").ReadOnly = True
            .Columns("emitente").ReadOnly = True
            .Columns("codFatura").ReadOnly = True
            .Columns("valorCte").ReadOnly = True
            .Columns("valorCteAtual").ReadOnly = True

        End With


    End Sub

    Private Sub Btn_ImportarXml_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Btn_ImportarXml.Click

        Call ConfiguraGrid()

        Dim dlg = New FolderBrowserDialog()
        Dim res As DialogResult = dlg.ShowDialog()
        Dim diretorio As String

        Cursor = Cursors.WaitCursor

        If res = DialogResult.OK Then
            diretorio = dlg.SelectedPath
        Else
            Exit Sub
        End If


        Dim dirDiretorio = New DirectoryInfo(diretorio)
        Dim oFileInfoCollection() As FileInfo
        Dim oFileInfo As FileInfo

        Cursor = Cursors.WaitCursor

        Dim xmlCte = New XmlDocument
        Dim listaCte = New List(Of CteAvonStralog)
        Dim cte As CteAvonStralog

        oFileInfoCollection = dirDiretorio.GetFiles("*.xml")

        If oFileInfoCollection.Length() = 0 Then
            lblStatus.Text = "Não foram localizados xml para importar!"
            Cursor = Cursors.Default
            Exit Sub
        End If

        If oFileInfoCollection.Length() > 0 Then

            Dim con = New Conexao()
            Dim dt As DataTable
            
            With grdNota

                If con.OpenConnection() Then

                    For i As Integer = 0 To oFileInfoCollection.Length() - 1

                        Try

                            lblStatus.Text = "Lendo " + (i + 1).ToString + " de " + oFileInfoCollection.Length.ToString

                            Application.DoEvents()

                            Cursor = Cursors.WaitCursor

                            oFileInfo = oFileInfoCollection.GetValue(i)

                            xmlCte.Load(oFileInfo.FullName)

                            cte = ExtrairDadosXml200(xmlCte)

                            If cte.AmbienteTeste Then Continue For

                            cte.Codigo = Val(Mid(cte.ChaveAcesso, 36, 8)).ToString()

                            If cte.Codigo < 42377 Then
                                Continue For
                            End If

                            If CDate(cte.DataEmissao) < CDate("2018-04-01") Then
                                Continue For
                            End If

                            If cte.ChaveAcesso = "" Then Continue For

                            If cte.ListaNFs.Count > 0 Then

                                con.Comando.CommandText = "Select idnfe, numero, numeroCte, ifnull(idcte,0) as idcte From tb_nfe Where (isnull(excluida) or excluida=false)" +
                                    " And idCte=" + cte.Codigo.ToString()

                                dt = con.ExecutaComandoDataTable()

                                If dt.Rows.Count = 0 Then

                                    con.Comando.CommandText = "Select ctrc_codigo, ctrc_numero, CTRC_CHAVEACESSO, ifnull(CTRC_CODFATURA,0) as CTRC_CODFATURA From tb_ctrc Where ctrc_codigo = " + cte.Codigo.ToString() +
                                            " And CTRC_SEFAZCODSTATUS='101'"

                                    Dim dtCte = con.ExecutaComandoDataTable

                                    If dtCte.Rows.Count > 0 Then
                                        Continue For
                                    End If

                                    con.Comando.CommandText = "Select idnfe, numero, numeroCte, ifnull(idcte,0) as idcte, nomeFantasiaEmitente, ifnull(valorTotalFrete, 0) as valorTotalFrete From tb_nfe Where (isnull(excluida) or excluida=false)" +
                                        " And numero=" + Val(Mid(cte.ListaNFs(0).ChaveAcesso, 26, 9)).ToString() +
                                        " And (isnull(reentrega) Or reentrega=false)"

                                    Dim dtCteAtual = con.ExecutaComandoDataTable()

                                    If dtCteAtual.Rows.Count = 0 Then

                                        Continue For

                                    ElseIf dtCteAtual.Rows.Count > 1 Then

                                        con.Comando.CommandText = "Select idnfe, numero, numeroCte, ifnull(idcte,0) as idcte, nomeFantasiaEmitente, ifnull(valorTotalFrete, 0) as valorTotalFrete From tb_nfe Where (isnull(excluida) or excluida=false)" +
                                            " And chaveacesso = '" + cte.ListaNFs(0).ChaveAcesso + "'" +
                                            " And (isnull(reentrega) Or reentrega=false)"
                                        dtCteAtual = con.ExecutaComandoDataTable()

                                    End If

                                    If dtCteAtual.Rows(0).Item("numeroCte").ToString() <> "" And dtCteAtual.Rows(0).Item("idcte").ToString() = "0" Then Continue For

                                    .Rows.Add()

                                    .Item("idCte", .RowCount - 1).Value = cte.Codigo
                                    .Item("numeroCte", .RowCount - 1).Value = cte.Numero
                                    .Item("chaveAcessoCte", .RowCount - 1).Value = cte.ChaveAcesso
                                    .Item("dataEmissaoCte", .RowCount - 1).Value = cte.DataEmissao
                                    .Item("idNfe", .RowCount - 1).Value = dtCteAtual.Rows(0).Item("idnfe")
                                    .Item("numeroNfe", .RowCount - 1).Value = dtCteAtual.Rows(0).Item("numero")
                                    .Item("chaveAcessoNfe", .RowCount - 1).Value = cte.ListaNFs(0).ChaveAcesso
                                    .Item("idCteAtual", .RowCount - 1).Value = dtCteAtual.Rows(0).Item("idcte").ToString()
                                    .Item("numeroCteAtual", .RowCount - 1).Value = dtCteAtual.Rows(0).Item("numeroCte").ToString()
                                    .Item("chaveAcessoCteAtual", .RowCount - 1).Value = "" 'dtCteAtual.rows(0).item("").tostring()
                                    .Item("emitente", .RowCount - 1).Value = dtCteAtual.Rows(0).Item("nomeFantasiaEmitente").ToString()
                                    .Item("codFatura", .RowCount - 1).Value = "" 'dtCteAtual.Rows(0).Item("CTRC_CODFATURA").ToString()
                                    .Item("valorCte", .RowCount - 1).Value = cte.ValorTotalFrete
                                    .Item("valorCteAtual", .RowCount - 1).Value = dtCteAtual.Rows(0).Item("valorTotalFrete").ToString()

                                End If

                            End If

                            Label1.Text = "NF-e encontradas " + .RowCount.ToString()

                            My.Application.DoEvents()

                            Cursor = Cursors.WaitCursor

                        Catch ex As Exception
                        End Try

                    Next

                    con.CloseConnection()

                End If

            End With

        End If

        Cursor = Cursors.Default

        MsgBox("Foram encontrados " + listaCte.Count.ToString + " CT-e's", MsgBoxStyle.Information)

    End Sub

    Private Function ExtrairDadosXml200(ByVal documentoXml As XmlDocument) As CteAvonStralog

        Dim cte = New CteAvonStralog

        Try


            For Each innerNodeCTe1 As XmlNode In documentoXml.ChildNodes
                For Each innerNodeCTe As XmlNode In innerNodeCTe1.ChildNodes
                    If innerNodeCTe.Name = "CTe" Then
                        For Each innerNode2 As XmlNode In innerNodeCTe.ChildNodes
                            If innerNode2.Name = "infCte" Then
                                If innerNode2.Attributes(0).Name = "Id" Then
                                    cte.ChaveAcesso = innerNode2.Attributes(0).InnerText.ToString.Replace("CTe", "")
                                End If
                                Try
                                    If innerNode2.Attributes(1).Name = "Id" Then
                                        cte.ChaveAcesso = innerNode2.Attributes(1).InnerText.ToString.Replace("CTe", "")
                                    End If
                                Catch ex As Exception
                                End Try
                                For Each innerNodeinfCTe As XmlNode In innerNode2.ChildNodes
                                    Select Case innerNodeinfCTe.Name
                                        Case "ide"
                                            For Each innerIde As XmlNode In innerNodeinfCTe.ChildNodes
                                                Select Case innerIde.Name
                                                    Case "serie"
                                                        cte.Serie = innerIde.InnerText
                                                    Case "nCT"
                                                        cte.Numero = innerIde.InnerText
                                                    Case "dhEmi"
                                                        cte.DataEmissao = CDate(innerIde.InnerText).ToString("dd/MM/yyyy")
                                                    Case "CFOP"
                                                        cte.cfop = innerIde.InnerText
                                                    Case "forPag"
                                                        cte.CodFormaPagamento = Val(innerIde.InnerText)
                                                    Case "tpCTe"
                                                        cte.TipoCte = Val(innerIde.InnerText)
                                                    Case "UFIni"
                                                        cte.UfColeta = innerIde.InnerText
                                                    Case "cMunIni"
                                                        cte.cMunIni = innerIde.InnerText
                                                    Case "UFFim"
                                                        cte.UfEntrega = innerIde.InnerText
                                                    Case "cMunFim"
                                                        cte.cMunFim = innerIde.InnerText
                                                    Case "tpAmb"
                                                        If innerIde.InnerText = "2" Then
                                                            cte.AmbienteTeste = True
                                                        Else
                                                            cte.AmbienteTeste = False
                                                        End If
                                                    Case "toma03"
                                                        For Each innerToma3 As XmlNode In innerIde
                                                            Select Case innerToma3.Name
                                                                Case "toma"
                                                                    Select Case innerIde.InnerText
                                                                        Case "0"
                                                                            cte.Pagador = CteAvon.PagadorFreteEnum.Remetente
                                                                            'cte.Pagador = "R"
                                                                        Case "1"
                                                                            cte.Pagador = CteAvon.PagadorFreteEnum.Expedidor
                                                                            'cte.Pagador = "E"
                                                                        Case "2"
                                                                            cte.Pagador = CteAvon.PagadorFreteEnum.Recebedor
                                                                            'cte.Pagador = "B"
                                                                        Case "3"
                                                                            cte.Pagador = CteAvon.PagadorFreteEnum.Destinatário
                                                                            'cte.Pagador = "D"
                                                                        Case "4"
                                                                            cte.Pagador = CteAvon.PagadorFreteEnum.Consignatário
                                                                            'cte.Pagador = "C"
                                                                    End Select
                                                            End Select
                                                        Next
                                                End Select
                                            Next
                                        Case "compl"
                                            For Each innerCompl As XmlNode In innerNodeinfCTe.ChildNodes
                                                Select Case innerCompl.Name
                                                    Case "xObs"
                                                        cte.MensagemFiscal = innerCompl.InnerText
                                                End Select
                                            Next

                                        Case "rem"
                                            For Each innerRem As XmlNode In innerNodeinfCTe.ChildNodes
                                                Select Case innerRem.Name
                                                    Case "CNPJ", "CPF"
                                                        cte.CadastroRemetente = innerRem.InnerText
                                                    Case "xNome"
                                                        cte.NomeRemetente = innerRem.InnerText
                                                    Case "IE"
                                                        cte.IeRementente = innerRem.InnerText
                                                    Case "email"
                                                        'cte.e = innerDest.InnerText
                                                    Case "enderReme"
                                                        For Each innerEnder As XmlNode In innerRem.ChildNodes
                                                            Select Case innerEnder.Name
                                                                Case "xLgr"
                                                                    cte.EnderecoRementente = Mid(innerEnder.InnerText, 1, 100)
                                                                Case "nro"
                                                                    cte.NumeroRemetente = Mid(innerEnder.InnerText, 1, 10)
                                                                Case "xCpl"
                                                                    'cte.Complemento = Mid(innerEnder.InnerText, 1, 50)
                                                                Case "xBairro"
                                                                    cte.BairroRementente = Mid(innerEnder.InnerText, 1, 40)
                                                                Case "cMun"
                                                                    cte.cMunFim = innerEnder.InnerText
                                                                    cte.CodCidadeRemetente = innerEnder.InnerText
                                                                Case "xMun"
                                                                    cte.NomeCidadeRemetente = Mid(innerEnder.InnerText, 1, 50).ToUpper
                                                                    cte.LocalColeta = Mid(innerEnder.InnerText, 1, 50).ToUpper
                                                                Case "UF"
                                                                    cte.UfRementente = innerEnder.InnerText
                                                                    cte.UfColeta = innerEnder.InnerText
                                                                Case "CEP"
                                                                    cte.CepRementente = innerEnder.InnerText
                                                                Case "fone"
                                                                    'cte.Telefone = Mid(ObjFunction.LimpaFormatacao(innerEnder.InnerText), 1, 15)
                                                                Case "cPais"
                                                                    'cte.CodPais = innerEnder.InnerText
                                                                Case "xPais"
                                                                    'ObjDestinatario.NomePais = innerEnder.InnerText
                                                            End Select
                                                        Next
                                                End Select
                                            Next

                                        Case "dest"
                                            For Each innerDest As XmlNode In innerNodeinfCTe.ChildNodes
                                                Select Case innerDest.Name
                                                    Case "CNPJ", "CPF"
                                                        cte.CadastroDestinatario = innerDest.InnerText
                                                    Case "xNome"
                                                        cte.NomeDestinatario = innerDest.InnerText
                                                    Case "IE"
                                                        cte.IeDestinatario = innerDest.InnerText
                                                    Case "email"
                                                        'cte.e = innerDest.InnerText
                                                    Case "enderDest"
                                                        For Each innerEnder As XmlNode In innerDest.ChildNodes
                                                            Select Case innerEnder.Name
                                                                Case "xLgr"
                                                                    cte.EnderecoDestinatario = Mid(innerEnder.InnerText, 1, 100)
                                                                Case "nro"
                                                                    cte.NumeroDestinatario = Mid(innerEnder.InnerText, 1, 10)
                                                                Case "xCpl"
                                                                    'cte.Complemento = Mid(innerEnder.InnerText, 1, 50)
                                                                Case "xBairro"
                                                                    cte.BairroDestinatario = Mid(innerEnder.InnerText, 1, 40)
                                                                Case "cMun"
                                                                    cte.cMunFim = innerEnder.InnerText
                                                                    cte.CodCidadeDestinatario = innerEnder.InnerText
                                                                Case "xMun"
                                                                    cte.NomeCidadeDestinatario = Mid(innerEnder.InnerText, 1, 50).ToUpper
                                                                    cte.LocalEntrega = Mid(innerEnder.InnerText, 1, 50).ToUpper
                                                                Case "UF"
                                                                    cte.UfDestinatario = innerEnder.InnerText
                                                                    cte.UfEntrega = innerEnder.InnerText
                                                                Case "CEP"
                                                                    cte.CepDestinatario = innerEnder.InnerText
                                                                Case "fone"
                                                                    'cte.Telefone = Mid(ObjFunction.LimpaFormatacao(innerEnder.InnerText), 1, 15)
                                                                Case "cPais"
                                                                    'cte.CodPais = innerEnder.InnerText
                                                                Case "xPais"
                                                                    'ObjDestinatario.NomePais = innerEnder.InnerText
                                                            End Select
                                                        Next
                                                End Select
                                            Next

                                        Case "vPrest"
                                            For Each innerVPrest As XmlNode In innerNodeinfCTe.ChildNodes
                                                Select Case innerVPrest.Name
                                                    Case "vTPrest"
                                                        cte.ValorTotalFrete = innerVPrest.InnerText.Replace(".", ",")
                                                    Case "Comp"
                                                        Dim composicao As String = ""
                                                        For Each innerComp As XmlNode In innerVPrest.ChildNodes
                                                            Select Case innerComp.Name
                                                                Case "xNome"
                                                                    composicao = innerComp.InnerText
                                                                Case "vComp"
                                                                    If composicao.ToUpper = "FRETE PESO" Then
                                                                        cte.ValorFretePeso = innerComp.InnerText.Replace(".", ",")
                                                                    ElseIf composicao.ToUpper = "FRETE VALOR" Then
                                                                        cte.ValorAdValorem = innerComp.InnerText.Replace(".", ",")
                                                                    ElseIf composicao.ToUpper = "FRETE ADVALOREN" Then
                                                                        cte.ValorGriss = innerComp.InnerText.Replace(".", ",")
                                                                    ElseIf composicao.ToUpper = "FRETE GRIS" Then
                                                                        cte.ValorGriss = innerComp.InnerText.Replace(".", ",")
                                                                    ElseIf composicao.ToUpper = "PEDAGIO" Or composicao.ToUpper = "PEDÁGIO" Then
                                                                        cte.ValorPedagio = innerComp.InnerText.Replace(".", ",")
                                                                    ElseIf composicao.ToUpper = "OUTROS" Or composicao.ToUpper = "VALOR OUTROS" Or composicao.ToUpper = "OUTRO" Then
                                                                        cte.ValorOutros = innerComp.InnerText.Replace(".", ",")
                                                                    ElseIf composicao.ToUpper = "TAXA TRT" Then
                                                                        cte.ValorTrt = innerComp.InnerText.Replace(".", ",")
                                                                    ElseIf composicao.ToUpper = "TAXA TDE" Then
                                                                        cte.ValorTde = innerComp.InnerText.Replace(".", ",")
                                                                    ElseIf composicao.ToUpper = "TRT" Then
                                                                        cte.ValorTrt = innerComp.InnerText.Replace(".", ",")
                                                                    ElseIf composicao.ToUpper = "TDE" Then
                                                                        cte.ValorTde = innerComp.InnerText.Replace(".", ",")
                                                                    ElseIf composicao.ToUpper = "IMPOSTOS" Then
                                                                        cte.ValorImpostoRepassado = innerComp.InnerText.Replace(".", ",")
                                                                    End If
                                                                    composicao = ""
                                                            End Select
                                                        Next
                                                End Select
                                            Next


                                        Case "infCTeNorm"
                                            Dim auxDadosNf As Integer = 1
                                            For Each innerCTeNorm As XmlNode In innerNodeinfCTe.ChildNodes
                                                Select Case innerCTeNorm.Name
                                                    Case "infCarga"
                                                        For Each innerCarga As XmlNode In innerCTeNorm.ChildNodes
                                                            Select Case innerCarga.Name
                                                                Case "vCarga"
                                                                    cte.ValorMercadoria = innerCarga.InnerText.Replace(".", ",")
                                                                Case "proPred"
                                                                    cte.NaturezaCarga = innerCarga.InnerText
                                                                Case "xOutCat"
                                                                    cte.EspecieCarga = innerCarga.InnerText
                                                                Case "infQ"
                                                                    Dim unidade As String = ""
                                                                    For Each innerQ As XmlNode In innerCarga.ChildNodes
                                                                        Select Case innerQ.Name
                                                                            Case "cUnid"
                                                                                unidade = innerQ.InnerText
                                                                            Case "qCarga"
                                                                                Select Case unidade
                                                                                    Case "01"
                                                                                        cte.PesoCarga = innerQ.InnerText.Replace(".", ",")
                                                                                        cte.PesoCubadoCarga = innerQ.InnerText.Replace(".", ",")
                                                                                    Case Else
                                                                                        cte.QuantidadeCarga = innerQ.InnerText.Replace(".", ",")
                                                                                End Select
                                                                        End Select
                                                                    Next

                                                            End Select
                                                        Next

                                                    Case "infDoc"
                                                        Dim listaCteNf = New List(Of CteNf)
                                                        For Each innerDoc As XmlNode In innerCTeNorm.ChildNodes
                                                            Select Case innerDoc.Name
                                                                Case "infNFe"
                                                                    For Each innerNf As XmlNode In innerDoc.ChildNodes
                                                                        Select Case innerNf.Name
                                                                            Case "chave"
                                                                                Dim cteNf = New CteNf
                                                                                cteNf.ChaveAcesso = innerNf.InnerText
                                                                                cteNf.Numero = Mid(cteNf.ChaveAcesso, 26, 9)
                                                                                cteNf.Serie = Mid(cteNf.ChaveAcesso, 23, 3)
                                                                                If Val(auxDadosNf.ToString).ToString() = "1" Then
                                                                                    cteNf.Quantidade = cte.QuantidadeCarga
                                                                                    cteNf.Peso = cte.PesoCarga
                                                                                    cteNf.Valor = cte.ValorMercadoria
                                                                                Else
                                                                                    cteNf.Quantidade = 0
                                                                                    cteNf.Peso = 0
                                                                                    cteNf.Valor = 0
                                                                                End If
                                                                                auxDadosNf += 1
                                                                                cteNf.DataEmissao = Mid(cteNf.ChaveAcesso, 3, 2) + "/" + Mid(cteNf.ChaveAcesso, 5, 2) + "/" + cte.DataEmissao.Year.ToString("0000")
                                                                                listaCteNf.Add(cteNf)
                                                                        End Select
                                                                    Next

                                                                Case "infNF"
                                                                    Dim cteNf = New CteNf
                                                                    For Each innerNf As XmlNode In innerDoc.ChildNodes
                                                                        Select Case innerNf.Name
                                                                            Case "serie"
                                                                                cteNf.Serie = innerNf.InnerText
                                                                            Case "nDoc"
                                                                                cteNf.Numero = innerNf.InnerText
                                                                                cteNf.Quantidade = 0
                                                                                cteNf.Peso = 0
                                                                            Case "vNF"
                                                                                cteNf.Valor = innerNf.InnerText.Replace(".", ",")
                                                                            Case "dEmi"
                                                                                cteNf.DataEmissao = innerNf.InnerText

                                                                        End Select
                                                                    Next
                                                                    listaCteNf.Add(cteNf)
                                                            End Select
                                                        Next
                                                        cte.ListaNFs = listaCteNf

                                                    Case "seg"
                                                        For Each innerSeg As XmlNode In innerCTeNorm.ChildNodes
                                                            Select Case innerSeg.Name
                                                                Case "respSeg"
                                                                    cte.RespSeguro = innerSeg.InnerText
                                                                Case "xSeg"
                                                                    cte.NomeSeguradora = innerSeg.InnerText
                                                                Case "nApol"
                                                                    cte.NumeroApolice = innerSeg.InnerText
                                                                Case "vCarga"
                                                                    cte.ValorCargaAverbada = innerSeg.InnerText.Replace(".", ",")
                                                            End Select
                                                        Next

                                                    Case "cobr"
                                                        For Each cobr As XmlNode In innerCTeNorm.ChildNodes
                                                            Select Case cobr.Name
                                                                Case "dup"
                                                                    For Each dup As XmlNode In cobr.ChildNodes
                                                                        If dup.Name = "dVenc" Then
                                                                            cte.DataVencimento = dup.InnerText
                                                                        End If
                                                                    Next
                                                            End Select
                                                        Next


                                                End Select
                                            Next

                                    End Select
                                Next
                            End If
                        Next

                    End If
                Next
            Next

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

        Return cte

    End Function

End Class

Public Class CteAvonStralog

    Inherits CteAvon

    Private _autorizado As Boolean
    Public Property Autorizado() As Boolean
        Get
            Return _autorizado
        End Get
        Set(ByVal value As Boolean)
            _autorizado = value
        End Set
    End Property

    Private _nProt As String
    Public Property nProt() As String
        Get
            Return _nProt
        End Get
        Set(ByVal value As String)
            _nProt = value
        End Set
    End Property

    Private _cfop As String
    Public Property cfop() As String
        Get
            Return _cfop
        End Get
        Set(ByVal value As String)
            _cfop = value
        End Set
    End Property

    Private _cMunIni As String
    Public Property cMunIni() As String
        Get
            Return _cMunIni
        End Get
        Set(ByVal value As String)
            _cMunIni = value
        End Set
    End Property

    Private _cMunFim As String
    Public Property cMunFim() As String
        Get
            Return _cMunFim
        End Get
        Set(ByVal value As String)
            _cMunFim = value
        End Set
    End Property

    Public Sub New()
        _cfop = ""
        _cMunIni = ""
        _cMunFim = ""
    End Sub


End Class

Public Class CteAvon

    Private _codEmpresa As Short
    Private _regCompartilhado As String
    Private _codigo As Int64
    Private _numero As Int64
    Private _serie As String
    Private _dataEmissao As Date
    Private _codCfop As Short
    Private _codRemetente As Int32
    Private _nomeRemetente As String
    Private _cadastroRemetente As String
    Private _ieRementente As String
    Private _enderecoRementente As String
    Private _numeroRemetente As String
    Private _cepRementente As String
    Private _bairroRementente As String
    Private _complementoRemetente As String
    Private _codCidadeRemetente As Int32
    Private _nomeCidadeRemetente As String
    Private _ufRementente As String
    Private _codDestinatario As Int32
    Private _nomeDestinatario As String
    Private _cadastroDestinatario As String
    Private _ieDestinatario As String
    Private _enderecoDestinatario As String
    Private _numeroDestinatario As String
    Private _cepDestinatario As String
    Private _bairroDestinatario As String
    Private _complementoDestinatario As String
    Private _codCidadeDestinatario As Int32
    Private _nomeCidadeDestinatario As String
    Private _ufDestinatario As String
    Private _codExpedidor As Integer
    Private _nomeExpedidor As String
    Private _cadastroExpedidor As String
    Private _ieExpedidor As String
    Private _enderecoExpedidor As String
    Private _numeroExpedidor As String
    Private _cepExpedidor As String
    Private _bairroExpedidor As String
    Private _complementoExpedidor As String
    Private _codCidadeExpedidor As Int32
    Private _nomeCidadeExpedidor As String
    Private _ufExpedidor As String
    Private _codRecebedor As Integer
    Private _nomeRecebedor As String
    Private _cadastroRecebedor As String
    Private _ieRecebedor As String
    Private _enderecoRecebedor As String
    Private _numeroRecebedor As String
    Private _cepRecebedor As String
    Private _bairroRecebedor As String
    Private _complementoRecebedor As String
    Private _codCidadeRecebedor As Int32
    Private _nomeCidadeRecebedor As String
    Private _ufRecebedor As String
    Private _pagador As PagadorFreteEnum
    Private _codPagador As Int32
    Private _codFormaPagamento As Short
    Private _codMotorista As Int32
    Private _codVeiculo As Integer
    Private _codTipoVeiculoCobrado As Int32
    Private _codTabelaFrete As Short
    Private _quantidadeCarga As Double
    Private _valorMercadoria As Decimal
    Private _pesoCarga As Double
    Private _pesoCubadoCarga As Double
    Private _porcentagemIcms As Double
    Private _valorIcms As Decimal
    Private _valorBaseCalculoIcms As Decimal
    Private _valorFretePeso As Decimal
    Private _valorFreteValor As Decimal
    Private _valorSecCat As Decimal
    Private _valorDespacho As Decimal
    Private _valorTaxas As Decimal
    Private _valorDiaria As Decimal
    Private _valorPedagio As Decimal
    Private _valorOutros As Decimal
    Private _valorAjudante As Decimal
    Private _valorGriss As Decimal
    Private _valorDescarga As Decimal
    Private _valorTde As Decimal
    Private _valorTrt As Decimal
    Private _valorTotalFrete As Decimal
    Private _mensagemFiscal As String
    Private _observacao As String
    Private _especieCarga As String
    Private _naturezaCarga As String
    Private _ufColeta As String
    Private _ufEntrega As String
    Private _localColeta As String
    Private _localEntrega As String
    Private _codCarreta As Integer
    Private _codCarreta2 As Integer
    Private _inclusoIcmsNoFrete As Boolean
    Private _dataEntrega As Date
    Private _codTipoTributacao As Short
    Private _respSeguro As Short
    Private _nomeSeguradora As String
    Private _numeroApolice As String
    Private _numeroAverbacao As String
    Private _valorCargaAverbada As Decimal
    Private _lotacao As Boolean
    Private _tipoCte As TipoCteEnum
    Private _tipoServico As TipoServicoEnum
    Private _codMinuta As Int64
    Private _chaveAcesso As String
    Private _protocolo As String
    Private _chaveCteComplementado As String
    Private _codFuncionarioEmissor As Short
    Private _numeroSequenciaAvon As String
    Private _porcentagemAdValoren As Decimal
    Private _valorAdValorem As Decimal
    Private _porcentagemGris As Decimal
    Private _listaCodNfMinuta As String
    Private _valorAposHorarioComercial As Decimal
    Private _valorKmExcedido As Decimal
    Private _valorForaPerimetro As Decimal
    Private _dataVencimento As Date
    Private _codConsignatario As Int32
    Private _nomeConsignatario As String
    Private _cadastroConsignatario As String
    Private _ieConsignatario As String
    Private _enderecoConsignatario As String
    Private _numeroConsignatario As String
    Private _cepConsignatario As String
    Private _bairroConsignatario As String
    Private _complementoConsignatario As String
    Private _codCidadeConsignatario As Int32
    Private _nomeCidadeConsignatario As String
    Private _ufConsignatario As String
    Private _valorImpostoRepassado As Double
    Private _enviaDadosMotoristaXml As Boolean
    Private _notasFiscais As String
    Private _qtdEntregas As Double
    Private _dataEntregaPrevisao As Date


    Private _listaNFs As List(Of CteNf)

    Private Sub InicializaVariaveis()

        _codEmpresa = 0
        _regCompartilhado = ""
        _codigo = 0
        _numero = 0
        _serie = ""
        _dataEmissao = Now
        _codCfop = 0
        _codRemetente = 0
        _nomeRemetente = ""
        _cadastroRemetente = ""
        _ieRementente = ""
        _enderecoRementente = ""
        _numeroRemetente = ""
        _cepRementente = ""
        _bairroRementente = ""
        _codCidadeRemetente = 0
        _nomeCidadeRemetente = ""
        _ufRementente = ""
        _codDestinatario = 0
        _nomeDestinatario = ""
        _cadastroDestinatario = ""
        _ieDestinatario = ""
        _enderecoDestinatario = ""
        _numeroDestinatario = ""
        _cepDestinatario = ""
        _bairroDestinatario = ""
        _complementoDestinatario = ""
        _complementoRemetente = ""
        _codCidadeDestinatario = 0
        _nomeCidadeDestinatario = ""
        _ufDestinatario = ""
        _codExpedidor = 0
        _nomeExpedidor = ""
        _cadastroExpedidor = ""
        _ieExpedidor = ""
        _enderecoExpedidor = ""
        _numeroExpedidor = ""
        _cepExpedidor = ""
        _bairroExpedidor = ""
        _complementoExpedidor = ""
        _complementoExpedidor = ""
        _codCidadeExpedidor = 0
        _nomeCidadeExpedidor = ""
        _ufExpedidor = ""
        _codRecebedor = 0
        _nomeRecebedor = ""
        _cadastroRecebedor = ""
        _ieRecebedor = ""
        _enderecoRecebedor = ""
        _numeroRecebedor = ""
        _cepRecebedor = ""
        _bairroRecebedor = ""
        _complementoRecebedor = ""
        _complementoRecebedor = ""
        _codCidadeRecebedor = 0
        _nomeCidadeRecebedor = ""
        _ufRecebedor = ""
        _pagador = PagadorFreteEnum.Remetente
        _codPagador = 0
        _codFormaPagamento = 0
        _codMotorista = 0
        _codVeiculo = 0
        _codTipoVeiculoCobrado = 0
        _codTabelaFrete = 0
        _quantidadeCarga = 0
        _valorMercadoria = 0
        _pesoCarga = 0
        _pesoCubadoCarga = 0
        _porcentagemIcms = 0
        _valorIcms = 0
        _valorBaseCalculoIcms = 0
        _valorFretePeso = 0
        _valorFreteValor = 0
        _valorSecCat = 0
        _valorDespacho = 0
        _valorTaxas = 0
        _valorDiaria = 0
        _valorPedagio = 0
        _valorOutros = 0
        _valorAjudante = 0
        _valorGriss = 0
        _valorDescarga = 0
        _valorTde = 0
        _valorTrt = 0
        _valorTotalFrete = 0
        _mensagemFiscal = ""
        _observacao = ""
        _especieCarga = ""
        _naturezaCarga = ""
        _ufColeta = ""
        _ufEntrega = ""
        _localColeta = ""
        _localEntrega = ""
        _codCarreta = 0
        _codCarreta2 = 0
        _inclusoIcmsNoFrete = False
        _dataEntrega = Now
        _codTipoTributacao = 0
        _respSeguro = 0
        _nomeSeguradora = ""
        _numeroApolice = ""
        _numeroAverbacao = ""
        _valorCargaAverbada = 0
        _lotacao = False
        _tipoCte = TipoCteEnum.Normal
        _tipoServico = TipoServicoEnum.Normal
        _codMinuta = 0
        _chaveAcesso = ""
        _protocolo = ""
        _chaveCteComplementado = ""
        _codFuncionarioEmissor = 0
        _numeroSequenciaAvon = ""
        _porcentagemAdValoren = 0
        _porcentagemGris = 0
        _valorAdValorem = 0
        _listaCodNfMinuta = "0"
        _valorAposHorarioComercial = 0
        _valorKmExcedido = 0
        _valorForaPerimetro = 0
        _valorImpostoRepassado = 0
        _enviaDadosMotoristaXml = True
        _dataVencimento = Nothing
        _codConsignatario = 0
        _nomeConsignatario = ""
        _cadastroConsignatario = ""
        _ieConsignatario = ""
        _enderecoConsignatario = ""
        _numeroConsignatario = ""
        _cepConsignatario = ""
        _bairroConsignatario = ""
        _complementoConsignatario = ""
        _codCidadeConsignatario = 0
        _nomeCidadeConsignatario = ""
        _ufConsignatario = ""
        _notasFiscais = ""
        _listaNFs = New List(Of CteNf)
        _qtdEntregas = 0
        _dataEntregaPrevisao = Now
        AmbienteTeste = False
    End Sub

    Public Sub New()
        Call InicializaVariaveis()
    End Sub

    Public Enum PagadorFreteEnum
        Remetente = 1
        Destinatário = 2
        Consignatário = 3
        Expedidor = 4
        Recebedor = 5
    End Enum

    Public Enum TipoCteEnum
        Normal = 0
        Complementar = 1
    End Enum

    Public Enum TipoServicoEnum
        Normal = 0
        SubContratação = 1
        Redespacho = 2
    End Enum

    Public Property ChaveAcessoCteComplentado() As String
        Get
            Return _chaveCteComplementado
        End Get
        Set(ByVal value As String)
            _chaveCteComplementado = value
        End Set
    End Property

    Public Property Pagador() As PagadorFreteEnum
        Get
            Return _pagador
        End Get
        Set(ByVal value As PagadorFreteEnum)
            _pagador = value
        End Set
    End Property

    Public Property TipoCte() As TipoCteEnum
        Get
            Return _tipoCte
        End Get
        Set(ByVal value As TipoCteEnum)
            _tipoCte = value
        End Set
    End Property

    Public Property TipoServico() As TipoServicoEnum
        Get
            Return _tipoServico
        End Get
        Set(ByVal value As TipoServicoEnum)
            _tipoServico = value
        End Set
    End Property

    Public Property CodEmpresa() As Short
        Get
            Return _codEmpresa
        End Get
        Set(ByVal value As Short)
            _codEmpresa = value
        End Set
    End Property

    Public Property RegCompartilhado() As String
        Get
            Return _regCompartilhado
        End Get
        Set(ByVal value As String)
            _regCompartilhado = value
        End Set
    End Property

    Public Property Codigo() As Long
        Get
            Return _codigo
        End Get
        Set(ByVal value As Long)
            _codigo = value
        End Set
    End Property

    Public Property Numero() As Int64
        Get
            Return _numero
        End Get
        Set(ByVal value As Int64)
            _numero = value
        End Set
    End Property

    Public Property Serie() As String
        Get
            Return _serie
        End Get
        Set(ByVal value As String)
            _serie = value
        End Set
    End Property

    Public Property DataEmissao() As Date
        Get
            Return _dataEmissao
        End Get
        Set(ByVal value As Date)
            _dataEmissao = value
        End Set
    End Property

    Public Property CodCfop() As Short
        Get
            Return _codCfop
        End Get
        Set(ByVal value As Short)
            _codCfop = value
        End Set
    End Property

    Public Property CodRemetente() As Integer
        Get
            Return _codRemetente
        End Get
        Set(ByVal value As Integer)
            _codRemetente = value
        End Set
    End Property

    Public Property NomeRemetente() As String
        Get
            Return _nomeRemetente
        End Get
        Set(ByVal value As String)
            _nomeRemetente = value
        End Set
    End Property

    Public Property CadastroRemetente() As String
        Get
            Return _cadastroRemetente
        End Get
        Set(ByVal value As String)
            _cadastroRemetente = value
        End Set
    End Property

    Public Property IeRementente() As String
        Get
            Return _ieRementente
        End Get
        Set(ByVal value As String)
            _ieRementente = value
        End Set
    End Property

    Public Property EnderecoRementente() As String
        Get
            Return _enderecoRementente
        End Get
        Set(ByVal value As String)
            _enderecoRementente = value
        End Set
    End Property

    Public Property NumeroRemetente() As String
        Get
            Return _numeroRemetente
        End Get
        Set(ByVal value As String)
            _numeroRemetente = value
        End Set
    End Property

    Public Property CepRementente() As String
        Get
            Return _cepRementente
        End Get
        Set(ByVal value As String)
            _cepRementente = value
        End Set
    End Property

    Public Property BairroRementente() As String
        Get
            Return _bairroRementente
        End Get
        Set(ByVal value As String)
            _bairroRementente = value
        End Set
    End Property

    Public Property ComplementoRemetente() As String
        Get
            Return _complementoRemetente
        End Get
        Set(ByVal value As String)
            _complementoRemetente = value
        End Set
    End Property

    Public Property CodCidadeRemetente() As Integer
        Get
            Return _codCidadeRemetente
        End Get
        Set(ByVal value As Integer)
            _codCidadeRemetente = value
        End Set
    End Property

    Public Property NomeCidadeRemetente() As String
        Get
            Return _nomeCidadeRemetente
        End Get
        Set(ByVal value As String)
            _nomeCidadeRemetente = value
        End Set
    End Property

    Public Property UfRementente() As String
        Get
            Return _ufRementente
        End Get
        Set(ByVal value As String)
            _ufRementente = value
        End Set
    End Property

    Public Property CodDestinatario() As Integer
        Get
            Return _codDestinatario
        End Get
        Set(ByVal value As Integer)
            _codDestinatario = value
        End Set
    End Property

    Public Property NomeDestinatario() As String
        Get
            Return _nomeDestinatario
        End Get
        Set(ByVal value As String)
            _nomeDestinatario = value
        End Set
    End Property

    Public Property CadastroDestinatario() As String
        Get
            Return _cadastroDestinatario
        End Get
        Set(ByVal value As String)
            _cadastroDestinatario = value
        End Set
    End Property

    Public Property IeDestinatario() As String
        Get
            Return _ieDestinatario
        End Get
        Set(ByVal value As String)
            _ieDestinatario = value
        End Set
    End Property

    Public Property EnderecoDestinatario() As String
        Get
            Return _enderecoDestinatario
        End Get
        Set(ByVal value As String)
            _enderecoDestinatario = value
        End Set
    End Property

    Public Property NumeroDestinatario() As String
        Get
            Return _numeroDestinatario
        End Get
        Set(ByVal value As String)
            _numeroDestinatario = value
        End Set
    End Property

    Public Property CepDestinatario() As String
        Get
            Return _cepDestinatario
        End Get
        Set(ByVal value As String)
            _cepDestinatario = value
        End Set
    End Property

    Public Property BairroDestinatario() As String
        Get
            Return _bairroDestinatario
        End Get
        Set(ByVal value As String)
            _bairroDestinatario = value
        End Set
    End Property

    Public Property ComplementoDestinatario() As String
        Get
            Return _complementoDestinatario
        End Get
        Set(ByVal value As String)
            _complementoDestinatario = value
        End Set
    End Property

    Public Property CodCidadeDestinatario() As Integer
        Get
            Return _codCidadeDestinatario
        End Get
        Set(ByVal value As Integer)
            _codCidadeDestinatario = value
        End Set
    End Property

    Public Property NomeCidadeDestinatario() As String
        Get
            Return _nomeCidadeDestinatario
        End Get
        Set(ByVal value As String)
            _nomeCidadeDestinatario = value
        End Set
    End Property

    Public Property UfDestinatario() As String
        Get
            Return _ufDestinatario
        End Get
        Set(ByVal value As String)
            _ufDestinatario = value
        End Set
    End Property

    Public Property CodExpedidor() As Integer
        Get
            Return _codExpedidor
        End Get
        Set(ByVal value As Integer)
            _codExpedidor = value
        End Set
    End Property
    Public Property NomeExpedidor() As String
        Get
            Return _nomeExpedidor
        End Get
        Set(ByVal value As String)
            _nomeExpedidor = value
        End Set
    End Property

    Public Property CadastroExpedidor() As String
        Get
            Return _cadastroExpedidor
        End Get
        Set(ByVal value As String)
            _cadastroExpedidor = value
        End Set
    End Property

    Public Property IeExpedidor() As String
        Get
            Return _ieExpedidor
        End Get
        Set(ByVal value As String)
            _ieExpedidor = value
        End Set
    End Property

    Public Property EnderecoExpedidor() As String
        Get
            Return _enderecoExpedidor
        End Get
        Set(ByVal value As String)
            _enderecoExpedidor = value
        End Set
    End Property

    Public Property NumeroExpedidor() As String
        Get
            Return _numeroExpedidor
        End Get
        Set(ByVal value As String)
            _numeroExpedidor = value
        End Set
    End Property

    Public Property CepExpedidor() As String
        Get
            Return _cepExpedidor
        End Get
        Set(ByVal value As String)
            _cepExpedidor = value
        End Set
    End Property

    Public Property BairroExpedidor() As String
        Get
            Return _bairroExpedidor
        End Get
        Set(ByVal value As String)
            _bairroExpedidor = value
        End Set
    End Property

    Public Property ComplementoExpedidor() As String
        Get
            Return _complementoExpedidor
        End Get
        Set(ByVal value As String)
            _complementoExpedidor = value
        End Set
    End Property

    Public Property CodCidadeExpedidor() As Integer
        Get
            Return _codCidadeExpedidor
        End Get
        Set(ByVal value As Integer)
            _codCidadeExpedidor = value
        End Set
    End Property

    Public Property NomeCidadeExpedidor() As String
        Get
            Return _nomeCidadeExpedidor
        End Get
        Set(ByVal value As String)
            _nomeCidadeExpedidor = value
        End Set
    End Property

    Public Property UfExpedidor() As String
        Get
            Return _ufExpedidor
        End Get
        Set(ByVal value As String)
            _ufExpedidor = value
        End Set
    End Property


    Public Property CodRecebedor() As Integer
        Get
            Return _codRecebedor
        End Get
        Set(ByVal value As Integer)
            _codRecebedor = value
        End Set
    End Property

    Public Property NomeRecebedor() As String
        Get
            Return _nomeRecebedor
        End Get
        Set(ByVal value As String)
            _nomeRecebedor = value
        End Set
    End Property

    Public Property AmbienteTeste() As Boolean

    Public Property CadastroRecebedor() As String
        Get
            Return _cadastroRecebedor
        End Get
        Set(ByVal value As String)
            _cadastroRecebedor = value
        End Set
    End Property

    Public Property IeRecebedor() As String
        Get
            Return _ieRecebedor
        End Get
        Set(ByVal value As String)
            _ieRecebedor = value
        End Set
    End Property

    Public Property EnderecoRecebedor() As String
        Get
            Return _enderecoRecebedor
        End Get
        Set(ByVal value As String)
            _enderecoRecebedor = value
        End Set
    End Property

    Public Property NumeroRecebedor() As String
        Get
            Return _numeroRecebedor
        End Get
        Set(ByVal value As String)
            _numeroRecebedor = value
        End Set
    End Property

    Public Property CepRecebedor() As String
        Get
            Return _cepRecebedor
        End Get
        Set(ByVal value As String)
            _cepRecebedor = value
        End Set
    End Property

    Public Property BairroRecebedor() As String
        Get
            Return _bairroRecebedor
        End Get
        Set(ByVal value As String)
            _bairroRecebedor = value
        End Set
    End Property

    Public Property ComplementoRecebedor() As String
        Get
            Return _complementoRecebedor
        End Get
        Set(ByVal value As String)
            _complementoRecebedor = value
        End Set
    End Property

    Public Property CodCidadeRecebedor() As Integer
        Get
            Return _codCidadeRecebedor
        End Get
        Set(ByVal value As Integer)
            _codCidadeRecebedor = value
        End Set
    End Property

    Public Property NomeCidadeRecebedor() As String
        Get
            Return _nomeCidadeRecebedor
        End Get
        Set(ByVal value As String)
            _nomeCidadeRecebedor = value
        End Set
    End Property

    Public Property UfRecebedor() As String
        Get
            Return _ufRecebedor
        End Get
        Set(ByVal value As String)
            _ufRecebedor = value
        End Set
    End Property

    Public Property CodPagador() As Integer
        Get
            Return _codPagador
        End Get
        Set(ByVal value As Integer)
            _codPagador = value
        End Set
    End Property

    Public Property CodFormaPagamento() As Short
        Get
            Return _codFormaPagamento
        End Get
        Set(ByVal value As Short)
            _codFormaPagamento = value
        End Set
    End Property

    Public Property CodMotorista() As Integer
        Get
            Return _codMotorista
        End Get
        Set(ByVal value As Integer)
            _codMotorista = value
        End Set
    End Property

    Public Property CodVeiculo() As Integer
        Get
            Return _codVeiculo
        End Get
        Set(ByVal value As Integer)
            _codVeiculo = value
        End Set
    End Property

    Public Property CodTipoVeiculoCobrado() As Integer
        Get
            Return _codTipoVeiculoCobrado
        End Get
        Set(ByVal value As Integer)
            _codTipoVeiculoCobrado = value
        End Set
    End Property

    Public Property CodTabelaFrete() As Short
        Get
            Return _codTabelaFrete
        End Get
        Set(ByVal value As Short)
            _codTabelaFrete = value
        End Set
    End Property

    Public Property QuantidadeCarga() As Double
        Get
            Return _quantidadeCarga
        End Get
        Set(ByVal value As Double)
            _quantidadeCarga = value
        End Set
    End Property

    Public Property ValorMercadoria() As Decimal
        Get
            Return _valorMercadoria
        End Get
        Set(ByVal value As Decimal)
            _valorMercadoria = value
        End Set
    End Property

    Public Property PesoCarga() As Double
        Get
            Return _pesoCarga
        End Get
        Set(ByVal value As Double)
            _pesoCarga = value
        End Set
    End Property

    Public Property PorcentagemIcms() As Double
        Get
            Return _porcentagemIcms
        End Get
        Set(ByVal value As Double)
            _porcentagemIcms = value
        End Set
    End Property

    Public Property ValorIcms() As Decimal
        Get
            Return _valorIcms
        End Get
        Set(ByVal value As Decimal)
            _valorIcms = value
        End Set
    End Property

    Public Property ValorBaseCalculoIcms() As Decimal
        Get
            Return _valorBaseCalculoIcms
        End Get
        Set(ByVal value As Decimal)
            _valorBaseCalculoIcms = value
        End Set
    End Property

    Public Property ValorFretePeso() As Decimal
        Get
            Return _valorFretePeso
        End Get
        Set(ByVal value As Decimal)
            _valorFretePeso = value
        End Set
    End Property

    Public Property ValorFreteValor() As Decimal
        Get
            Return _valorFreteValor
        End Get
        Set(ByVal value As Decimal)
            _valorFreteValor = value
        End Set
    End Property

    Public Property ValorSecCat() As Decimal
        Get
            Return _valorSecCat
        End Get
        Set(ByVal value As Decimal)
            _valorSecCat = value
        End Set
    End Property

    Public Property ValorDespacho() As Decimal
        Get
            Return _valorDespacho
        End Get
        Set(ByVal value As Decimal)
            _valorDespacho = value
        End Set
    End Property

    Public Property ValorTaxas() As Decimal
        Get
            Return _valorTaxas
        End Get
        Set(ByVal value As Decimal)
            _valorTaxas = value
        End Set
    End Property

    Public Property ValorDiaria() As Decimal
        Get
            Return _valorDiaria
        End Get
        Set(ByVal value As Decimal)
            _valorDiaria = value
        End Set
    End Property

    Public Property ValorPedagio() As Decimal
        Get
            Return _valorPedagio
        End Get
        Set(ByVal value As Decimal)
            _valorPedagio = value
        End Set
    End Property

    Public Property ValorOutros() As Decimal
        Get
            Return _valorOutros
        End Get
        Set(ByVal value As Decimal)
            _valorOutros = value
        End Set
    End Property

    Public Property ValorAjudante() As Decimal
        Get
            Return _valorAjudante
        End Get
        Set(ByVal value As Decimal)
            _valorAjudante = value
        End Set
    End Property

    Public Property ValorGriss() As Decimal
        Get
            Return _valorGriss
        End Get
        Set(ByVal value As Decimal)
            _valorGriss = value
        End Set
    End Property

    Public Property ValorDescarga() As Decimal
        Get
            Return _valorDescarga
        End Get
        Set(ByVal value As Decimal)
            _valorDescarga = value
        End Set
    End Property

    Public Property ValorTotalFrete() As Decimal
        Get
            Return _valorTotalFrete
        End Get
        Set(ByVal value As Decimal)
            _valorTotalFrete = value
        End Set
    End Property

    Public Property MensagemFiscal() As String
        Get
            Return _mensagemFiscal
        End Get
        Set(ByVal value As String)
            _mensagemFiscal = value
        End Set
    End Property

    Public Property Observacao() As String
        Get
            Return _observacao
        End Get
        Set(ByVal value As String)
            _observacao = value
        End Set
    End Property

    Public Property EspecieCarga() As String
        Get
            Return _especieCarga
        End Get
        Set(ByVal value As String)
            _especieCarga = value
        End Set
    End Property

    Public Property NaturezaCarga() As String
        Get
            Return _naturezaCarga
        End Get
        Set(ByVal value As String)
            _naturezaCarga = value
        End Set
    End Property

    Public Property UfColeta() As String
        Get
            Return _ufColeta
        End Get
        Set(ByVal value As String)
            _ufColeta = value
        End Set
    End Property

    Public Property UfEntrega() As String
        Get
            Return _ufEntrega
        End Get
        Set(ByVal value As String)
            _ufEntrega = value
        End Set
    End Property

    Public Property LocalColeta() As String
        Get
            Return _localColeta
        End Get
        Set(ByVal value As String)
            _localColeta = value
        End Set
    End Property

    Public Property LocalEntrega() As String
        Get
            Return _localEntrega
        End Get
        Set(ByVal value As String)
            _localEntrega = value
        End Set
    End Property

    Public Property CodCarreta() As Integer
        Get
            Return _codCarreta
        End Get
        Set(ByVal value As Integer)
            _codCarreta = value
        End Set
    End Property

    Public Property CodCarreta2() As Integer
        Get
            Return _codCarreta2
        End Get
        Set(ByVal value As Integer)
            _codCarreta2 = value
        End Set
    End Property

    Public Property InclusoIcmsNoFrete() As Boolean
        Get
            Return _inclusoIcmsNoFrete
        End Get
        Set(ByVal value As Boolean)
            _inclusoIcmsNoFrete = value
        End Set
    End Property

    Public Property DataEntrega() As Date
        Get
            Return _dataEntrega
        End Get
        Set(ByVal value As Date)
            _dataEntrega = value
        End Set
    End Property

    Public Property DataEntregaPrevisao() As Date
        Get
            Return _dataEntregaPrevisao
        End Get
        Set(ByVal value As Date)
            _dataEntregaPrevisao = value
        End Set
    End Property

    Public Property CodTipoTributacao() As Short
        Get
            Return _codTipoTributacao
        End Get
        Set(ByVal value As Short)
            _codTipoTributacao = value
        End Set
    End Property

    Public Property RespSeguro() As Short
        Get
            Return _respSeguro
        End Get
        Set(ByVal value As Short)
            _respSeguro = value
        End Set
    End Property

    Public Property NomeSeguradora() As String
        Get
            Return _nomeSeguradora
        End Get
        Set(ByVal value As String)
            _nomeSeguradora = value
        End Set
    End Property

    Public Property NumeroApolice() As String
        Get
            Return _numeroApolice
        End Get
        Set(ByVal value As String)
            _numeroApolice = value
        End Set
    End Property

    Public Property NumeroAverbacao() As String
        Get
            Return _numeroAverbacao
        End Get
        Set(ByVal value As String)
            _numeroAverbacao = value
        End Set
    End Property

    Public Property ValorCargaAverbada() As Decimal
        Get
            Return _valorCargaAverbada
        End Get
        Set(ByVal value As Decimal)
            _valorCargaAverbada = value
        End Set
    End Property

    Public Property Lotacao() As Boolean
        Get
            Return _lotacao
        End Get
        Set(ByVal value As Boolean)
            _lotacao = value
        End Set
    End Property

    Public Property CodMinuta() As Long
        Get
            Return _codMinuta
        End Get
        Set(ByVal value As Long)
            _codMinuta = value
        End Set
    End Property

    Public Property ChaveAcesso() As String
        Get
            Return _chaveAcesso
        End Get
        Set(ByVal value As String)
            _chaveAcesso = value
        End Set
    End Property

    Public Property Protocolo() As String
        Get
            Return _protocolo
        End Get
        Set(ByVal value As String)
            _protocolo = value
        End Set
    End Property
    Public Property CodFuncionarioEmissor() As Short
        Get
            Return _codFuncionarioEmissor
        End Get
        Set(ByVal value As Short)
            _codFuncionarioEmissor = value
        End Set
    End Property

    Public Property NumeroSequenciaAvon() As String
        Get
            Return _numeroSequenciaAvon
        End Get
        Set(ByVal value As String)
            _numeroSequenciaAvon = value
        End Set
    End Property

    Public Property PorcentagemAdValoren() As Decimal
        Get
            Return _porcentagemAdValoren
        End Get
        Set(ByVal value As Decimal)
            _porcentagemAdValoren = value
        End Set
    End Property

    Public Property PorcentagemGris() As Decimal
        Get
            Return _porcentagemGris
        End Get
        Set(ByVal value As Decimal)
            _porcentagemGris = value
        End Set
    End Property

    Public Property ValorAdValorem() As Decimal
        Get
            Return _valorAdValorem
        End Get
        Set(ByVal value As Decimal)
            _valorAdValorem = value
        End Set
    End Property

    Public Property ListaCodNfMinuta() As String
        Get
            Return _listaCodNfMinuta
        End Get
        Set(ByVal value As String)
            _listaCodNfMinuta = value
        End Set
    End Property

    Public Property ValorAposHorarioComercial() As Decimal
        Get
            Return _valorAposHorarioComercial
        End Get
        Set(ByVal value As Decimal)
            _valorAposHorarioComercial = value
        End Set
    End Property

    Public Property ValorKmExcedido() As Decimal
        Get
            Return _valorKmExcedido
        End Get
        Set(ByVal value As Decimal)
            _valorKmExcedido = value
        End Set
    End Property

    Public Property ValorForaPerimetro() As Decimal
        Get
            Return _valorForaPerimetro
        End Get
        Set(ByVal value As Decimal)
            _valorForaPerimetro = value
        End Set
    End Property

    Public Property ValorImpostoRepassado() As Double
        Get
            Return _valorImpostoRepassado
        End Get
        Set(ByVal value As Double)
            _valorImpostoRepassado = value
        End Set
    End Property

    Public Property EnviaDadosMotoristaXml() As Boolean
        Get
            Return _enviaDadosMotoristaXml
        End Get
        Set(ByVal value As Boolean)
            _enviaDadosMotoristaXml = value
        End Set
    End Property

    Public Property DataVencimento() As Date
        Get
            Return _dataVencimento
        End Get
        Set(ByVal value As Date)
            _dataVencimento = value
        End Set
    End Property

    Public Property ValorTde() As Decimal
        Get
            Return _valorTde
        End Get
        Set(ByVal value As Decimal)
            _valorTde = value
        End Set
    End Property

    Public Property ValorTrt() As Decimal
        Get
            Return _valorTrt
        End Get
        Set(ByVal value As Decimal)
            _valorTrt = value
        End Set
    End Property

    Public Property PesoCubadoCarga() As Double
        Get
            Return _pesoCubadoCarga
        End Get
        Set(ByVal value As Double)
            _pesoCubadoCarga = value
        End Set
    End Property

    Public Property ListaNFs() As List(Of CteNf)
        Get
            Return _listaNFs
        End Get
        Set(ByVal value As List(Of CteNf))
            _listaNFs = value
        End Set
    End Property

    Public Property CodConsignatario() As Integer
        Get
            Return _codConsignatario
        End Get
        Set(ByVal value As Integer)
            _codConsignatario = value
        End Set
    End Property

    Public Property NomeConsignatario() As String
        Get
            Return _nomeConsignatario
        End Get
        Set(ByVal value As String)
            _nomeConsignatario = value
        End Set
    End Property

    Public Property CadastroConsignatario() As String
        Get
            Return _cadastroConsignatario
        End Get
        Set(ByVal value As String)
            _cadastroConsignatario = value
        End Set
    End Property
    Public Property IeConsignatario() As String
        Get
            Return _ieConsignatario
        End Get
        Set(ByVal value As String)
            _ieConsignatario = value
        End Set
    End Property
    Public Property EnderecoConsignatario() As String
        Get
            Return _enderecoConsignatario
        End Get
        Set(ByVal value As String)
            _enderecoConsignatario = value
        End Set
    End Property

    Public Property NumeroConsignatario() As String
        Get
            Return _numeroConsignatario
        End Get
        Set(ByVal value As String)
            _numeroConsignatario = value
        End Set
    End Property
    Public Property CepConsignatario() As String
        Get
            Return _cepConsignatario
        End Get
        Set(ByVal value As String)
            _cepConsignatario = value
        End Set
    End Property
    Public Property BairroConsignatario() As String
        Get
            Return _bairroConsignatario
        End Get
        Set(ByVal value As String)
            _bairroConsignatario = value
        End Set
    End Property
    Public Property ComplementoConsignatario() As String
        Get
            Return _complementoConsignatario
        End Get
        Set(ByVal value As String)
            _complementoConsignatario = value
        End Set
    End Property
    Public Property CodCidadeConsignatario() As Integer
        Get
            Return _codCidadeConsignatario
        End Get
        Set(ByVal value As Integer)
            _codCidadeConsignatario = value
        End Set
    End Property
    Public Property NomeCidadeConsignatario() As String
        Get
            Return _nomeCidadeConsignatario
        End Get
        Set(ByVal value As String)
            _nomeCidadeConsignatario = value
        End Set
    End Property
    Public Property UfConsignatario() As String
        Get
            Return _ufConsignatario
        End Get
        Set(ByVal value As String)
            _ufConsignatario = value
        End Set
    End Property

    Public Property NotasFiscais() As String
        Get
            Return _notasFiscais
        End Get
        Set(ByVal value As String)
            _notasFiscais = value
        End Set
    End Property

    Public Property QtdEntregas() As Double
        Get
            Return _qtdEntregas
        End Get
        Set(ByVal value As Double)
            _qtdEntregas = value
        End Set
    End Property

End Class

Public Class CteNf

    Private _empCodigo As Short
    Private _regCompartilhado As String
    Private _codigo As Int64
    Private _codCte As Int64
    Private _numero As Int64
    Private _serie As String
    Private _quantidade As Double
    Private _peso As Double
    Private _valor As Double
    Private _dataEmissao As Date
    Private _chaveAcesso As String
    Private _valorFrete As Decimal

    Private Sub InicializaVariaveis()
        _empCodigo = 0
        _regCompartilhado = "N"
        _codigo = 0
        _codCte = 0
        _numero = 0
        _serie = ""
        _quantidade = 0
        _peso = 0
        _valor = 0
        _dataEmissao = Now
        _chaveAcesso = ""
        _valorFrete = 0
    End Sub

    Public Sub New()
        Call InicializaVariaveis()
    End Sub

    Public Property CodCte() As Long
        Get
            Return _codCte
        End Get
        Set(ByVal value As Long)
            _codCte = value
        End Set
    End Property

    Public Property ValorFrete() As Decimal
        Get
            Return _valorFrete
        End Get
        Set(ByVal value As Decimal)
            _valorFrete = value
        End Set
    End Property

    Public Property ChaveAcesso() As String
        Get
            Return _chaveAcesso
        End Get
        Set(ByVal value As String)
            _chaveAcesso = value
        End Set
    End Property

    Public Property DataEmissao() As Date
        Get
            Return _dataEmissao
        End Get
        Set(ByVal value As Date)
            _dataEmissao = value
        End Set
    End Property

    Public Property Valor() As Double
        Get
            Return _valor
        End Get
        Set(ByVal value As Double)
            _valor = value
        End Set
    End Property

    Public Property Peso() As Double
        Get
            Return _peso
        End Get
        Set(ByVal value As Double)
            _peso = value
        End Set
    End Property

    Public Property Quantidade() As Double
        Get
            Return _quantidade
        End Get
        Set(ByVal value As Double)
            _quantidade = value
        End Set
    End Property

    Public Property Serie() As String
        Get
            Return _serie
        End Get
        Set(ByVal value As String)
            _serie = value
        End Set
    End Property

    Public Property Numero() As Long
        Get
            Return _numero
        End Get
        Set(ByVal value As Long)
            _numero = value
        End Set
    End Property

    Public Property Codigo() As Long
        Get
            Return _codigo
        End Get
        Set(ByVal value As Long)
            _codigo = value
        End Set
    End Property

    Public Property RegCompartilhado() As String
        Get
            Return _regCompartilhado
        End Get
        Set(ByVal value As String)
            _regCompartilhado = value
        End Set
    End Property

    Public Property EmpCodigo() As Short
        Get
            Return _empCodigo
        End Get
        Set(ByVal value As Short)
            _empCodigo = value
        End Set
    End Property

End Class