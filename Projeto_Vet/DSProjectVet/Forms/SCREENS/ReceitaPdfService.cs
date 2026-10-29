
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp;

namespace DSProjectVet.Forms.SCREENS
{
    public static class ReceitaPdfService
    {
        // PDFsharp 6.5 requer seleção de licença.
        // Este inicializador executa apenas uma vez.
        static ReceitaPdfService()
        {
            PdfSharpLicense.SetAgplLicense();
        }

        // ======================================
        // GERAR PDF
        // ======================================

        public static void Gerar(
            int idConsulta,
            string connectionString,
            string caminhoArquivo)
        {
            if (idConsulta <= 0)
                throw new ArgumentException(
                    "Consulta inválida.");

            DataTable cabecalho;
            DataTable prescricoes;

            using (SqlConnection conexao =
                new SqlConnection(connectionString))
            {
                conexao.Open();

                // DADOS DO ATENDIMENTO
                string sqlCabecalho = @"
                    SELECT
                        c.ID_Consulta,
                        c.DataHora,
                        a.Nome AS Animal,
                        cl.Nome AS Tutor,
                        f.Nome AS Veterinario,
                        f.CRMV
                    FROM dbo.Consultas AS c

                    LEFT JOIN dbo.Animais AS a
                        ON a.ID_Animal =
                           c.fk_Animais_ID_Animal

                    LEFT JOIN dbo.Clientes AS cl
                        ON cl.ID_Cliente =
                           a.fk_Clientes_ID_Cliente

                    LEFT JOIN dbo.Funcionarios AS f
                        ON f.ID_Funcionario =
                           c.fk_Funcionarios_ID_Funcionario

                    WHERE c.ID_Consulta = @Consulta;
                ";

                // MEDICAMENTOS PRESCRITOS
                string sqlPrescricoes = @"
                    SELECT
                        p.ID_Prescricao,
                        m.Nome AS Medicamento,
                        p.Quantidade,
                        p.Dosagem,
                        p.Frequencia,
                        p.Duracao,
                        p.Observacoes
                    FROM dbo.Prescricao AS p

                    INNER JOIN dbo.Medicamentos AS m
                        ON m.ID_Medicamento =
                           p.fk_Medicamentos_ID_Medicamento

                    WHERE p.fk_Consultas_ID_Consulta =
                          @Consulta

                    ORDER BY p.ID_Prescricao;
                ";

                cabecalho = Consultar(
                    conexao, sqlCabecalho, idConsulta);

                prescricoes = Consultar(
                    conexao, sqlPrescricoes, idConsulta);
            }

            if (cabecalho.Rows.Count != 1)
            {
                throw new Exception(
                    "A consulta não foi encontrada.");
            }

            if (prescricoes.Rows.Count == 0)
            {
                throw new Exception(
                    "Esta consulta não possui " +
                    "medicamentos prescritos.");
            }

            DataRow consulta = cabecalho.Rows[0];

            // ======================================
            // DOCUMENTO PDF
            // ======================================

            Document documento = new Document();

            documento.Info.Title =
                "Receita Veterinária - Consulta " +
                idConsulta;

            documento.Info.Author = "Med Vet";

            documento.Info.Subject =
                "Prescrição veterinária";

            var estiloNormal =
                documento.Styles["Normal"];

            estiloNormal.Font.Name = "Arial";
            estiloNormal.Font.Size = 10;

            estiloNormal.ParagraphFormat.SpaceAfter =
                Unit.FromPoint(5);

            Section secao = documento.AddSection();

            secao.PageSetup.PageFormat = PageFormat.A4;

            secao.PageSetup.TopMargin =
                Unit.FromCentimeter(1.9);

            secao.PageSetup.BottomMargin =
                Unit.FromCentimeter(2.0);

            secao.PageSetup.LeftMargin =
                Unit.FromCentimeter(2.2);

            secao.PageSetup.RightMargin =
                Unit.FromCentimeter(2.2);

            // ======================================
            // MARCA
            // ======================================

            Paragraph marca =
                secao.AddParagraph("MED VET");

            marca.Format.Font.Size = 22;

            marca.Format.Font.Bold = true;

            marca.Format.Font.Color =
                Color.FromRgb(20, 192, 138);

            marca.Format.SpaceAfter =
                Unit.FromPoint(2);

            Paragraph subtitulo = secao.AddParagraph(
                "SISTEMA DE GESTÃO VETERINÁRIA");

            subtitulo.Format.Font.Size = 8;

            subtitulo.Format.Font.Color =
                Color.FromRgb(105, 118, 112);

            // LINHA DIVISÓRIA
            Paragraph divisoria = secao.AddParagraph();

            divisoria.Format.Borders.Bottom.Width =
                Unit.FromPoint(1.5);

            divisoria.Format.Borders.Bottom.Color =
                Color.FromRgb(20, 192, 138);

            divisoria.Format.SpaceAfter =
                Unit.FromPoint(16);

            // ======================================
            // TÍTULO
            // ======================================

            Paragraph titulo = secao.AddParagraph(
                "RECEITUÁRIO VETERINÁRIO");

            titulo.Format.Font.Size = 15;

            titulo.Format.Font.Bold = true;

            titulo.Format.Font.Color =
                Color.FromRgb(35, 49, 45);

            titulo.Format.SpaceAfter =
                Unit.FromPoint(13);

            // ======================================
            // IDENTIFICAÇÃO
            // ======================================

            AdicionarCampo(
                secao, "Consulta: ",
                "#" + idConsulta.ToString("D4"));

            AdicionarCampo(
                secao, "Paciente: ",
                Texto(consulta, "Animal"));

            AdicionarCampo(
                secao, "Tutor / responsável: ",
                Texto(consulta, "Tutor"));

            AdicionarCampo(
                secao, "Profissional: ",
                Texto(consulta, "Veterinario"));

            AdicionarCampo(
                secao, "CRMV: ",
                Texto(consulta, "CRMV"));

            if (consulta["DataHora"] != DBNull.Value)
            {
                AdicionarCampo(
                    secao,
                    "Data da consulta: ",
                    Convert.ToDateTime(
                        consulta["DataHora"])
                        .ToString("dd/MM/yyyy HH:mm"));
            }

            AdicionarCampo(
                secao,
                "Emissão do documento: ",
                DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

            secao.AddParagraph();

            // ======================================
            // MEDICAMENTOS
            // ======================================

            Paragraph tituloMedicamentos =
                secao.AddParagraph(
                    "MEDICAMENTOS PRESCRITOS");

            tituloMedicamentos.Format.Font.Size = 12;

            tituloMedicamentos.Format.Font.Bold = true;

            tituloMedicamentos.Format.Font.Color =
                Color.FromRgb(14, 145, 104);

            tituloMedicamentos.Format.SpaceAfter =
                Unit.FromPoint(8);

            int numero = 1;

            foreach (DataRow linha in prescricoes.Rows)
            {
                Paragraph tituloMedicamento =
                    secao.AddParagraph(
                        numero.ToString("D2") + "  -  " +
                        Texto(linha, "Medicamento"));

                tituloMedicamento.Format.Font.Size = 12;

                tituloMedicamento.Format.Font.Bold = true;

                tituloMedicamento.Format.SpaceBefore =
                    Unit.FromPoint(12);

                tituloMedicamento.Format.SpaceAfter =
                    Unit.FromPoint(5);

                tituloMedicamento.Format.KeepWithNext =
                    true;

                AdicionarCampo(
                    secao,
                    "Quantidade: ",
                    Texto(linha, "Quantidade") +
                    " unidade(s)");

                AdicionarCampo(
                    secao,
                    "Dosagem: ",
                    Texto(linha, "Dosagem"));

                AdicionarCampo(
                    secao,
                    "Frequência: ",
                    Texto(linha, "Frequencia"));

                AdicionarCampo(
                    secao,
                    "Duração: ",
                    Texto(linha, "Duracao"));

                string observacoes =
                    Texto(linha, "Observacoes");

                if (observacoes != "Não informado")
                {
                    AdicionarCampo(
                        secao,
                        "Observações: ",
                        observacoes);
                }

                Paragraph separador =
                    secao.AddParagraph();

                separador.Format.Borders.Bottom.Width =
                    Unit.FromPoint(0.4);

                separador.Format.Borders.Bottom.Color =
                    Color.FromRgb(225, 232, 229);

                separador.Format.SpaceAfter =
                    Unit.FromPoint(5);

                numero++;
            }

            // ======================================
            // ASSINATURA
            // ======================================

            secao.AddParagraph();

            Paragraph linhaAssinatura =
                secao.AddParagraph(
                    "________________________________________");

            linhaAssinatura.Format.Alignment =
                ParagraphAlignment.Center;

            linhaAssinatura.Format.SpaceBefore =
                Unit.FromPoint(30);

            Paragraph nomeVeterinario =
                secao.AddParagraph(
                    Texto(consulta, "Veterinario"));

            nomeVeterinario.Format.Alignment =
                ParagraphAlignment.Center;

            nomeVeterinario.Format.Font.Bold = true;

            Paragraph crmv =
                secao.AddParagraph(
                    "CRMV: " + Texto(consulta, "CRMV"));

            crmv.Format.Alignment =
                ParagraphAlignment.Center;

            Paragraph aviso =
                secao.AddParagraph(
                    "Documento para conferência e " +
                    "assinatura do médico-veterinário.");

            aviso.Format.Alignment =
                ParagraphAlignment.Center;

            aviso.Format.Font.Size = 8;

            aviso.Format.Font.Color =
                Color.FromRgb(110, 120, 116);

            aviso.Format.SpaceBefore =
                Unit.FromPoint(8);

            // ======================================
            // RODAPÉ DAS PÁGINAS
            // ======================================

            Paragraph rodape =
                secao.Footers.Primary.AddParagraph(
                    "Med Vet  |  Prescrição da consulta #" +
                    idConsulta.ToString("D4"));

            rodape.Format.Font.Size = 8;

            rodape.Format.Font.Color =
                Color.FromRgb(130, 140, 135);

            rodape.Format.Alignment =
                ParagraphAlignment.Center;

            // ======================================
            // RENDERIZAR E SALVAR
            // ======================================

            PdfDocumentRenderer renderizador =
                new PdfDocumentRenderer();

            renderizador.Document = documento;

            renderizador.RenderDocument();

            renderizador.Save(caminhoArquivo);
        }

        // ======================================
        // CONSULTA PARAMETRIZADA
        // ======================================

        private static DataTable Consultar(
            SqlConnection conexao,
            string sql,
            int idConsulta)
        {
            DataTable tabela = new DataTable();

            using (SqlCommand comando =
                new SqlCommand(sql, conexao))
            using (SqlDataAdapter adaptador =
                new SqlDataAdapter(comando))
            {
                comando.Parameters.Add(
                    "@Consulta",
                    SqlDbType.Int).Value =
                        idConsulta;

                comando.CommandTimeout = 30;

                adaptador.Fill(tabela);
            }

            return tabela;
        }

        // ======================================
        // CAMPOS DE TEXTO DO PDF
        // ======================================

        private static string Texto(
            DataRow linha,
            string coluna)
        {
            if (!linha.Table.Columns.Contains(coluna))
                return "Não informado";

            if (linha[coluna] == DBNull.Value)
                return "Não informado";

            string valor =
                Convert.ToString(linha[coluna]).Trim();

            return string.IsNullOrWhiteSpace(valor)
                ? "Não informado"
                : valor;
        }

        private static void AdicionarCampo(
            Section secao,
            string titulo,
            string valor)
        {
            Paragraph p = secao.AddParagraph();

            p.AddFormattedText(
                titulo,
                TextFormat.Bold);

            string texto =
                string.IsNullOrWhiteSpace(valor)
                    ? "Não informado"
                    : valor;

            string[] linhas = texto
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Split('\n');

            for (int i = 0; i < linhas.Length; i++)
            {
                if (i > 0)
                    p.AddLineBreak();

                p.AddText(linhas[i]);
            }

            p.Format.SpaceAfter =
                Unit.FromPoint(5);
        }
    }
}
