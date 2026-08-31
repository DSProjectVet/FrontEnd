using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSProjectVet
{
    public static class DesignerUtils
    {
        // O "this" transforma o método em um Extension Method (Método de Extensão)
        public static void btnPrimary(this Button botao)
        {
            botao.AccessibleRole = AccessibleRole.Default;
            botao.AllowDrop = false;
            botao.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            botao.AutoEllipsis = false;
            botao.AutoSize = false;
            botao.AutoSizeMode = AutoSizeMode.GrowOnly;

            // Cores e Fonte baseadas na sua imagem
            botao.BackColor = Color.FromArgb(16, 187, 130);
            botao.BackgroundImage = null;
            botao.BackgroundImageLayout = ImageLayout.Tile;
            botao.ForeColor = SystemColors.Window;
            botao.Font = new Font("Microsoft Tai Le", 9F, FontStyle.Bold);

            // Estilo e Cursor
            botao.CausesValidation = true;
            botao.ContextMenuStrip = null;
            botao.Cursor = Cursors.Hand;
            botao.DialogResult = DialogResult.None;
            botao.Dock = DockStyle.None;
            botao.Enabled = true;
            botao.FlatStyle = FlatStyle.Flat;

            // Remove a borda preta padrão do estilo Flat (opcional, deixa mais moderno)
            botao.FlatAppearance.BorderSize = 0;
        }
    }
}
