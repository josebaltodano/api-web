using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Libreria
{
    public partial class LibrosForm : Form
    {
        private static int id = 0;
        public LibrosForm()
        {
            InitializeComponent();
        }

        private void LibrosForm_Load(object sender, EventArgs e)
        {
            GetAllLibros();
        }

        private async void GetAllLibros()
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync("https://localhost:44343/api/Libros"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var LibrosJsonString = await response.Content.ReadAsStringAsync();
                        dgvLibros.DataSource = JsonConvert.DeserializeObject<List<ViewModels.LibrosViewModel>>(LibrosJsonString)
                            .ToList();
                    }
                    else
                    {
                        MessageBox.Show("No se pueden obtener los libros: " + response.StatusCode);
                    }
                }
            }
        }

        private async void AddLibro()
        {
            ViewModels.LibrosViewModel oLibro = new ViewModels.LibrosViewModel();
            oLibro.ISBN = txtISBN.Text;
            oLibro.Titulo = txtTitulo.Text;
            oLibro.Autor = txtAutor.Text;
            oLibro.Temas = txtTemas.Text;
            oLibro.Editorial = txtEditorial.Text;
            using (var client = new HttpClient())
            {
                var serializedLibro = JsonConvert.SerializeObject(oLibro);
                var content = new StringContent(serializedLibro, Encoding.UTF8, "application/json");
                var result = await client.PostAsync("https://localhost:44343/api/Libros", content);
                if (result.IsSuccessStatusCode)
                    MessageBox.Show("Libro Registrado Correctamente");
                else
                    MessageBox.Show("Error al registrar el libro" +  result.Content.ReadAsStringAsync().Result);

            }
            Limpiar();
            GetAllLibros();
        }

        private void Limpiar()
        {
            txtISBN.Text = String.Empty;
            txtAutor.Text = String.Empty;
            txtTitulo.Text = String.Empty;
            txtTemas.Text = String.Empty;
            txtEditorial.Text = String.Empty;
            id = 0;
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            AddLibro();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if(id != 0)
            {
                UpdateLibro(id);
            }
        }

        private async void UpdateLibro(int id)
        {
            ViewModels.LibrosViewModel olibro = new ViewModels.LibrosViewModel();
            olibro.Id = id;
            olibro.ISBN = txtISBN.Text;
            olibro.Autor = txtAutor.Text;
            olibro.Temas = txtTemas.Text;
            olibro.Titulo = txtTitulo.Text;
            olibro.Editorial = txtEditorial.Text;
            using(var client = new HttpClient())
            {
                var serializeobject = JsonConvert.SerializeObject(olibro);
                var content = new StringContent(serializeobject, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await client.PutAsync("https://localhost:44343/api/Libros" + olibro.Id, content);
                if (responseMessage.IsSuccessStatusCode)
                    MessageBox.Show("Registro Actualizado");
                else
                    MessageBox.Show("Error al Actualizar el Registro"+responseMessage.StatusCode);
            }
        }

        private void dgvLibros_CellClick(object sender, DataGridViewCellEventArgs e)
        {
             foreach(DataGridViewRow row in dgvLibros.Rows)
            {
                if(row.Index == e.RowIndex)
                {
                    id = int.Parse(row.Cells[0].Value.ToString());
                    GetAllLibrosById(id);
                }
            }
        }

        private  async void GetAllLibrosById(int id)
        {
            using(var client = new HttpClient())
            {
                string URL = "https://localhost:44343/api/Libros/" + id.ToString();
                HttpResponseMessage response = await client.GetAsync(URL);
                if (response.IsSuccessStatusCode)
                {
                    var lisbrosJsonString = await response.Content.ReadAsStringAsync();
                    ViewModels.LibrosViewModel olibro = JsonConvert.DeserializeObject<ViewModels.LibrosViewModel>(lisbrosJsonString);
                    txtISBN.Text = olibro.ISBN;
                    txtAutor.Text = olibro.Autor;
                    txtTemas.Text = olibro.Temas;
                    txtTitulo.Text = olibro.Titulo;
                    txtEditorial.Text = olibro.Editorial;
                }
                else
                {
                    MessageBox.Show("No se puede obtener el libro" + response.StatusCode);
                }
            }
      
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if(id != 0)
            {
                Deletelibro(id);
            }
        }

        private async void Deletelibro(int id)
        {
            using (var client= new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44343/api/Libros");
                HttpResponseMessage response = await client.DeleteAsync(string.Format("{0}/{1}", "https://localhost:44343/api/Libros", id));
                if (response.IsSuccessStatusCode)
                    MessageBox.Show("Libro Eliminado");
                else
                    MessageBox.Show("No se pudo Borrar El libro:"+response.StatusCode);
            }
            Limpiar();
            GetAllLibros();
        }
    }
}
