Imports MySql.Data.MySqlClient

    Public Class Conexao

        ' ReSharper disable UnassignedField.Local
        Private _myTransaction As MySqlTransaction
        ' ReSharper restore UnassignedField.Local

        Private _command As MySqlCommand
        Private _myReader As MySqlDataReader
        Private _connection As MySqlConnection

        Private myDataAdapter As MySqlDataAdapter

        Public Property Comando() As MySqlCommand
            Get
                Return _command
            End Get
            Set(value As MySqlCommand)
                _command = value
            End Set
        End Property

        Public Property Reader() As MySqlDataReader
            Get
                Return _myReader
            End Get
            Set(value As MySqlDataReader)
                _myReader = value
            End Set
        End Property

        Public Property Connection() As MySqlConnection
            Get
                Return _connection
            End Get
            Set(value As MySqlConnection)
                _connection = value
            End Set
        End Property

        Public Sub Script(ByVal sql As String, Optional ByVal delimiter As String = "")
            Try

                Dim script As MySqlScript = New MySqlScript(_connection, sql)
                If delimiter <> "" Then
                    script.Delimiter = delimiter
                End If

                script.Execute()

            Catch ex As Exception
                Throw
            End Try

        End Sub

        Public Property Conexao() As MySqlConnection
            Get
                Return _connection
            End Get
            Set(value As MySqlConnection)
                _connection = value
            End Set
        End Property

        Private Sub Inicializa()

            Dim connectionString = "SERVER=dds-17;DATABASE=db_qualityfast;UID=ddsinfo;PWD=dds21231;Port=3307;sslmode=none"

            Try

                _connection = New MySqlConnection(connectionString)

                _command = New MySqlCommand()

                _command.Connection = _connection

            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            End Try

        End Sub

        Public Sub New()
            Call Inicializa()
        End Sub

        'open connection to database
        Public Function OpenConnection() As Boolean
            Try

                if _connection.State=ConnectionState.Open Then Return true

                _connection.Open()

                Return True

            Catch ex As Exception
                Return False
            End Try
        End Function

        'Close connection
        Public Function CloseConnection() As Boolean
            Try

                if _connection.State=ConnectionState.Closed Then Return true

                _connection.Close()

                Return True

            Catch ex As Exception
                Return False
            End Try
        End Function

        'Open tansaction
        Public Function OpenTransaction() As Boolean
            Try
                _myTransaction = _connection.BeginTransaction()
                Comando.Transaction = _myTransaction
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Function RollBanckTransaction() As Boolean
            Try
                _myTransaction.Rollback()
            Catch ex As Exception
            End Try
            Return True
        End Function

        'Commit transaction
        Public Function CommitTransaction() As Boolean
            Try
                _myTransaction.Commit()
            Catch ex As Exception
            End Try
            Return True
        End Function

        Public ReadOnly Property ExecutaComandoDataTable() As DataTable
        Get

            myDataAdapter = New MySqlDataAdapter(Comando)

            Dim dt = New DataTable

            Try

                MyDataAdapter.Fill(dt)

            Catch erro As MySqlException
                Throw erro
                'Cls_TextFile.GeraLogErro(Err.Number, Err.Description, "CarregaDataSet", "Cls_ControlTable")
            End Try
            Return dt
        End Get
    End Property

        ''' <summary>
        ''' Adiciona um vetor de parametros ao MySqlCommand.
        ''' </summary>
        ''' <param name="valores">O vetor de strings que deve ser adicionada aos parametros.</param>
        ''' <param name="paramNomeBase">Nome dado ao parametro base. Este valor envolta de {} na query (CommandText) vai ser trocado.</param>
        ''' <example>SELECT * FROM table WHERE field IN ({paramNomeBase})</example>
        ''' <param name="separador">A string utilizada para separar os parametros na query.</param>
        ''' <returns>Use o retorno para a parte da query que utiliza o IN.</returns>
        ''' <remarks></remarks>

        Public Function AddArrayParameters(ByVal valores As IEnumerable, ByVal paramNomeBase As String, Optional ByVal separador As String = ", ") As MySqlParameter()

            Dim parametros As New List(Of MySqlParameter)
            Dim parametrosNome As New List(Of String)
            Dim paramNumero As Integer = 1

            For Each valor In valores
                Dim paramName = String.Format("@{0}{1}", paramNomeBase, paramNumero)
                parametrosNome.Add(paramName)
                parametros.Add(Comando.Parameters.AddWithValue(paramName, valor))

                paramNumero += 1
            Next

            Comando.CommandText = Comando.CommandText.Replace("{" + paramNomeBase + "}", String.Join(separador, parametrosNome))

            Return parametros.ToArray()

        End Function

        'End Function

        'Public Function TesteRelatorioDinamico(ByVal relatorioExtensaoEnum As Utilitarios.RelatorioExtensaoEnum,
        '                                  ByVal nomeRelatorio As String, ByVal local As String, ByVal formula As String,
        '                                  Optional ByVal listaFormulas As List(Of Item) = Nothing,
        '                                  Optional ByVal rd As ReportDocument = Nothing) As Stream

        '    If rd Is Nothing Then
        '        rd = New ReportDocument()
        '        rd.Load(Path.Combine(local, nomeRelatorio))
        '    End If

        '    'For Each crTable In rd.Database.Tables
        '    '    crTable.Location = _database + "." + crTable.Location.Substring(crTable.Location.LastIndexOf(".") + 1)
        '    'Next

        '    'If rd.Subreports.Count > 0 Then
        '    '    For I As Short = 0 To rd.Subreports.Count - 1
        '    '        For Each crTable In rd.Subreports(I).Database.Tables
        '    '            crTable.Location = _database + "." +
        '    '                               crTable.Location.Substring(crTable.Location.LastIndexOf(".") + 1)
        '    '        Next
        '    '    Next
        '    'End If

        '    rd.Refresh()
        '    rd.VerifyDatabase()

        '    If _command.Parameters.Count > 0 Then
        '        For Each parametros As MySqlParameter In _command.Parameters
        '            rd.SetParameterValue(parametros.ParameterName, parametros.Value)
        '        Next
        '    End If

        '    Try
        '        If listaFormulas.Count > 0 Then
        '            For Each item As Item In listaFormulas
        '                rd.DataDefinition.FormulaFields(item.Texto).Text = "'" + item.Extra + "'"
        '            Next
        '        End If
        '    Catch ex As Exception
        '    End Try

        '    Try

        '        Select Case relatorioExtensaoEnum
        '            Case Utilitarios.RelatorioExtensaoEnum.Pdf
        '                Return rd.ExportToStream(ExportFormatType.PortableDocFormat)
        '            Case Utilitarios.RelatorioExtensaoEnum.Excel
        '                Return rd.ExportToStream(ExportFormatType.Excel)
        '            Case Else
        '                Return rd.ExportToStream(ExportFormatType.WordForWindows)
        '        End Select

        '    Catch ex As Exception
        '        Throw
        '    End Try

        'End Function

    End Class
