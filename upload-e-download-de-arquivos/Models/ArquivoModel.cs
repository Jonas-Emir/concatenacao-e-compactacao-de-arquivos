namespace upload_e_download_de_arquivos.Models
{
    public class ArquivoModel
    {
        public int Id_Arquivo { get; set; }
        public string Descricao { get; set; }
        public byte[] Dados { get; set; }
        public string ContentType { get; set; }
        public DateTime DataEnvio { get; set; }
    }
}
