
using System;
using System.Data;
using System.Data.SqlClient;

namespace DSProjectVet.Forms.SCREENS
{
    public static class FinanceiroSync
    {
        public static void Sincronizar(
            SqlConnection conexao,
            SqlTransaction transacao,
            int idConsulta)
        {
            if (conexao == null ||
                conexao.State != ConnectionState.Open)
            {
                throw new InvalidOperationException(
                    "A conexão SQL deve estar aberta.");
            }

            if (transacao == null)
            {
                throw new InvalidOperationException(
                    "A sincronização financeira precisa " +
                    "da mesma transação da consulta.");
            }

            using (SqlCommand comando =
                new SqlCommand(
                    "dbo.sp_SincronizarFinanceiroConsulta",
                    conexao,
                    transacao))
            {
                comando.CommandType =
                    CommandType.StoredProcedure;

                comando.CommandTimeout = 30;

                comando.Parameters.Add(
                    "@Consulta",
                    SqlDbType.Int).Value =
                        idConsulta;

                comando.ExecuteNonQuery();
            }
        }
    }
}
