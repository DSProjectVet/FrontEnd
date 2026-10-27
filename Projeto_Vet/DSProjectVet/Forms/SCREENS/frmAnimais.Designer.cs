namespace DSProjectVet.Forms.SCREENS
{
    partial class frmAnimais
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAnimais));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nomeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.racaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sexoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.enfermidadeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descricaoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.especieDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fkClientesIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.animaisBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSetClinicaVetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSet_ClinicaVet = new DSProjectVet.Dados.DataSet_ClinicaVet();
            this.animaisTableAdapter = new DSProjectVet.Dados.DataSet_ClinicaVetTableAdapters.AnimaisTableAdapter();
            this.label1 = new System.Windows.Forms.Label();
            this.lblNome = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblCpf = new System.Windows.Forms.Label();
            this.btnEditar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.animaisBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetClinicaVetBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet_ClinicaVet)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDDataGridViewTextBoxColumn,
            this.nomeDataGridViewTextBoxColumn,
            this.racaDataGridViewTextBoxColumn,
            this.sexoDataGridViewTextBoxColumn,
            this.enfermidadeDataGridViewTextBoxColumn,
            this.descricaoDataGridViewTextBoxColumn,
            this.especieDataGridViewTextBoxColumn,
            this.fkClientesIDDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.animaisBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(373, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(654, 659);
            this.dataGridView1.TabIndex = 0;
            // 
            // iDDataGridViewTextBoxColumn
            // 
            this.iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            this.iDDataGridViewTextBoxColumn.HeaderText = "ID";
            this.iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            this.iDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nomeDataGridViewTextBoxColumn
            // 
            this.nomeDataGridViewTextBoxColumn.DataPropertyName = "Nome";
            this.nomeDataGridViewTextBoxColumn.HeaderText = "Nome";
            this.nomeDataGridViewTextBoxColumn.Name = "nomeDataGridViewTextBoxColumn";
            this.nomeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // racaDataGridViewTextBoxColumn
            // 
            this.racaDataGridViewTextBoxColumn.DataPropertyName = "Raca";
            this.racaDataGridViewTextBoxColumn.HeaderText = "Raca";
            this.racaDataGridViewTextBoxColumn.Name = "racaDataGridViewTextBoxColumn";
            this.racaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // sexoDataGridViewTextBoxColumn
            // 
            this.sexoDataGridViewTextBoxColumn.DataPropertyName = "Sexo";
            this.sexoDataGridViewTextBoxColumn.HeaderText = "Sexo";
            this.sexoDataGridViewTextBoxColumn.Name = "sexoDataGridViewTextBoxColumn";
            this.sexoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // enfermidadeDataGridViewTextBoxColumn
            // 
            this.enfermidadeDataGridViewTextBoxColumn.DataPropertyName = "Enfermidade";
            this.enfermidadeDataGridViewTextBoxColumn.HeaderText = "Enfermidade";
            this.enfermidadeDataGridViewTextBoxColumn.Name = "enfermidadeDataGridViewTextBoxColumn";
            this.enfermidadeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // descricaoDataGridViewTextBoxColumn
            // 
            this.descricaoDataGridViewTextBoxColumn.DataPropertyName = "Descricao";
            this.descricaoDataGridViewTextBoxColumn.HeaderText = "Descricao";
            this.descricaoDataGridViewTextBoxColumn.Name = "descricaoDataGridViewTextBoxColumn";
            this.descricaoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // especieDataGridViewTextBoxColumn
            // 
            this.especieDataGridViewTextBoxColumn.DataPropertyName = "Especie";
            this.especieDataGridViewTextBoxColumn.HeaderText = "Especie";
            this.especieDataGridViewTextBoxColumn.Name = "especieDataGridViewTextBoxColumn";
            this.especieDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // fkClientesIDDataGridViewTextBoxColumn
            // 
            this.fkClientesIDDataGridViewTextBoxColumn.DataPropertyName = "fk_Clientes_ID";
            this.fkClientesIDDataGridViewTextBoxColumn.HeaderText = "fk_Clientes_ID";
            this.fkClientesIDDataGridViewTextBoxColumn.Name = "fkClientesIDDataGridViewTextBoxColumn";
            this.fkClientesIDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // animaisBindingSource
            // 
            this.animaisBindingSource.DataMember = "Animais";
            this.animaisBindingSource.DataSource = this.dataSetClinicaVetBindingSource;
            // 
            // dataSetClinicaVetBindingSource
            // 
            this.dataSetClinicaVetBindingSource.DataSource = this.dataSet_ClinicaVet;
            this.dataSetClinicaVetBindingSource.Position = 0;
            // 
            // dataSet_ClinicaVet
            // 
            this.dataSet_ClinicaVet.DataSetName = "DataSet_ClinicaVet";
            this.dataSet_ClinicaVet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // animaisTableAdapter
            // 
            this.animaisTableAdapter.ClearBeforeFill = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(30, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 21);
            this.label1.TabIndex = 6;
            this.label1.Text = "Animais do cliente:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // lblNome
            // 
            this.lblNome.AutoSize = true;
            this.lblNome.BackColor = System.Drawing.Color.Transparent;
            this.lblNome.Font = new System.Drawing.Font("Microsoft Tai Le", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNome.ForeColor = System.Drawing.Color.Black;
            this.lblNome.Location = new System.Drawing.Point(29, 68);
            this.lblNome.Name = "lblNome";
            this.lblNome.Size = new System.Drawing.Size(71, 27);
            this.lblNome.TabIndex = 5;
            this.lblNome.Text = "label1";
            this.lblNome.Click += new System.EventHandler(this.lblNome_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(30, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 21);
            this.label2.TabIndex = 7;
            this.label2.Text = "CPF do cliente:";
            // 
            // lblCpf
            // 
            this.lblCpf.AutoSize = true;
            this.lblCpf.BackColor = System.Drawing.Color.Transparent;
            this.lblCpf.Font = new System.Drawing.Font("Microsoft Tai Le", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCpf.ForeColor = System.Drawing.Color.Black;
            this.lblCpf.Location = new System.Drawing.Point(29, 138);
            this.lblCpf.Name = "lblCpf";
            this.lblCpf.Size = new System.Drawing.Size(71, 27);
            this.lblCpf.TabIndex = 8;
            this.lblCpf.Text = "label1";
            this.lblCpf.Click += new System.EventHandler(this.lblCpf_Click);
            // 
            // btnEditar
            // 
            this.btnEditar.Location = new System.Drawing.Point(34, 182);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(170, 38);
            this.btnEditar.TabIndex = 9;
            this.btnEditar.Text = "Editar Animal";
            this.btnEditar.UseVisualStyleBackColor = true;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            // frmAnimais
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(1039, 697);
            this.Controls.Add(this.btnEditar);
            this.Controls.Add(this.lblCpf);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblNome);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAnimais";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Animais";
            this.Load += new System.EventHandler(this.frmAnimais_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.animaisBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetClinicaVetBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet_ClinicaVet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource dataSetClinicaVetBindingSource;
        private Dados.DataSet_ClinicaVet dataSet_ClinicaVet;
        private System.Windows.Forms.BindingSource animaisBindingSource;
        private Dados.DataSet_ClinicaVetTableAdapters.AnimaisTableAdapter animaisTableAdapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nomeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn racaDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sexoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn enfermidadeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descricaoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn especieDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fkClientesIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblNome;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblCpf;
        private System.Windows.Forms.Button btnEditar;
    }
}