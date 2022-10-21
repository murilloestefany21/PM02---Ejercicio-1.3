using System;
using AppSQLite.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppSQLite.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    
    public partial class ShowPersons : ContentPage
    {
        // Instancia global de Objeto Personas para reutilizarlo en diferentes metodos
        private Personas temporalPerson = new Personas();
        // string dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PM02.db3");

        public ShowPersons()
        {
            InitializeComponent();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            chargeListView();
        }
        private async void chargeListView()
        {

            var ListViewPersons = await App.BaseDatos.getListPersons();
            // Si la lista no esta vacia, cargar los elementos de ListViewPersons a listPersons que es un ListView que esta en el ShowPersons.xaml
            if(ListViewPersons!=null)
            {
                listPersons.ItemsSource = ListViewPersons;
            }
        }

        private async void listPersons_ItemSelected(object sender, SelectedItemChangedEventArgs eventListener)
        {
            // Obtener el objeto o item seleccionado del ListView
            var itemSelected = (Personas) eventListener.SelectedItem;

            // Mostrar los botones de actualizacion y eliminacion una vez se seleccione un elemento del ListView
            btnUpdatePerson.IsVisible = true;
            btnDeletePerson.IsVisible = true;

            // Definir en una variable el valor del id o codigo del elemento seleccionado
            var idPersonSelected = itemSelected.code;

            // Si el id de la persona seleccionada es distinto de cero proceder.
            if (idPersonSelected != 0) 
            {
                // Obtener los datos de la persona mediante el metodo de obtener la persona por medio del id del elemento seleccionado del ListView
                var personObtained = await App.BaseDatos.getPersonByCode(idPersonSelected);
                if(personObtained != null)
                {
                    
                    // Asignando los valores correspondientes de la persona obtenida de la BD a la instancia global del objeto Personas (Linea 20).
                    temporalPerson.code = personObtained.code;
                    temporalPerson.firstNames = personObtained.firstNames;
                    temporalPerson.lastNames = personObtained.lastNames;
                    temporalPerson.age = personObtained.age;
                    temporalPerson.address = personObtained.address;
                    temporalPerson.email = personObtained.email;
            
                }
            }

        }

        private async void btnUpdatePerson_Clicked(object sender, EventArgs e)
        {
            // Crear una instancia de la ventana de Actualizar persona
            var secondPage = new UpdatePerson();

            // Enlanzando los datos del objeto Personas temporal para enviarlo a la ventana de Actualizacion
            secondPage.BindingContext = temporalPerson;

            // Antes de pasar a la ventana de Actualizacion volvemos a ocultar los botones de actualizacion y eliminacion
            btnUpdatePerson.IsVisible = false;
            btnDeletePerson.IsVisible = false;
            await Navigation.PushAsync(secondPage);
            
            // await Navigation.PushAsync(new UpdatePerson());
        }

        private async void btnDeletePerson_Clicked(object sender, EventArgs e)
        {
            // Obtenemos el objeto personas de la BD para usarlo en el metodo de eliminar
            var personObtained = await App.BaseDatos.getPersonByCode(temporalPerson.code);

            if(personObtained != null)
            {
                // Borrando el registro de la BD
                await App.BaseDatos.deletePerson(personObtained);

                await DisplayAlert("Eliminar", "El registro se elimino correctamente", "OK");

                // Una vez borrado el registro y cerrado el DisplayAlert volvemos a ocultar los botones de Actualizacion y Elminar
                btnUpdatePerson.IsVisible = false;
                btnDeletePerson.IsVisible = false;
                chargeListView();
            }
            
        }
    }
}