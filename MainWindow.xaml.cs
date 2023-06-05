using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Remoting.Messaging;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using static System.Net.WebRequestMethods;

namespace Var4_Policlinica
{
    public class SortItem
    {
        public string DisplayName { get; set; }
        public string PropertyName { get; set; }
        public bool Ascending { get; set; }

    }
    public partial class MainWindow : Window
    {

        public static Var4_PoliclinicaEntities connection = new Var4_PoliclinicaEntities();

        public static ObservableCollection<Patient> Patients { get; set; }

        public static ObservableCollection<Doctor> Doctors { get; set; }

        public static ObservableCollection<Specialization> Specializations { get; set; }

        public static ObservableCollection<Reception> Receptions { get; set; }

        public static ObservableCollection<Treatment> Treatments { get; set; }

        private Patient selectedPat = null;

        private Doctor selectedDoc = null;

        private Reception selectedRec = null;

        private Treatment selectedTre = null;


        public ObservableCollection<SortItem> SortItems { get; set; } = new ObservableCollection<SortItem>()
        {
            new SortItem() { DisplayName = "По возрастанию Ф.И.О.", PropertyName = "FullName", Ascending = true},
            new SortItem() { DisplayName = "По убыванию Ф.И.О.", PropertyName = "FullName", Ascending = false}
           

        };
        public MainWindow()
        {
            InitializeComponent();
            Patients = new ObservableCollection<Patient>(connection.Patient.ToList());
            Doctors = new ObservableCollection<Doctor>(connection.Doctor.ToList());
            Specializations = new ObservableCollection<Specialization>(connection.Specialization.ToList());
            Receptions = new ObservableCollection<Reception>(connection.Reception.ToList());
            Treatments = new ObservableCollection<Treatment>(connection.Treatment.ToList());
            Filter_Spec.ItemsSource = connection.Specialization.ToList();
            Filter_Spec_Pr.ItemsSource = connection.Specialization.ToList();
            Filter_Spec_Tre.ItemsSource = connection.Specialization.ToList();
            DataContext = this;
            LoadSpec();
           
        }


       

        private void SortByName(string property, bool asc = true)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(List_Doctor.ItemsSource);
            if (view == null) return;

            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(property, asc ? ListSortDirection.Ascending : ListSortDirection.Descending));
        }


        private void LoadSpec()
        {
            var spec = connection.Specialization.ToList();
            foreach (var sp in spec)
            {
                Spec_CB.Items.Add(sp.Name);

            }
        }


        private void FilterBySpec(string Specialization)
        {
            ICollectionView view_2 = CollectionViewSource.GetDefaultView(List_Doctor.ItemsSource);
            if (view_2 == null) return;

            view_2.Filter = new Predicate<object>(obj =>
            {
                if (Specialization == "Все типы")
                    return true;
                return ((Doctor)obj).Specialization == Specialization;
            });

        }

        

        private void SearchBy_P(string substring)
        {
            ICollectionView view_3 = CollectionViewSource.GetDefaultView(List_Patient.ItemsSource);
            if (view_3 == null) return;
            view_3.Filter = new Predicate<object>(obj =>
            {


                return
                ((Patient)obj).Medical_policy.ToLower().Contains(substring.ToLower());
                


            });

        }

        private void SearchBy_Rec(string substring)
        {
            ICollectionView view_3 = CollectionViewSource.GetDefaultView(List_Reception.ItemsSource);
            if (view_3 == null) return;
            view_3.Filter = new Predicate<object>(obj =>
            {


                return
                ((Reception)obj).Doctor1.FullName.ToLower().Contains(substring.ToLower());
            });

        }


        private void SearchBy_Tre(string substring)
        {
            ICollectionView view_3 = CollectionViewSource.GetDefaultView(List_Treatment.ItemsSource);
            if (view_3 == null) return;
            view_3.Filter = new Predicate<object>(obj =>
            {


                return
                ((Treatment)obj).Doctor1.FullName.ToLower().Contains(substring.ToLower());
            });

        }


        private void SearchBy_D(string substring)
        {
            ICollectionView view_3 = CollectionViewSource.GetDefaultView(List_Doctor.ItemsSource);
            if (view_3 == null) return;
            view_3.Filter = new Predicate<object>(obj =>
            {


                return
                ((Doctor)obj).Phone.ToLower().Contains(substring.ToLower());



            });

        }

        private void Add_Btn_Click(object sender, RoutedEventArgs e)
        {
            //var pat = new Patient();

            selectedPat = new Patient();

            if (FIO_TB.Text.Length == 0)
            {
                MessageBox.Show("Введите Ф.И.О.");
                return;
            }

            if (Birthday_DP.Text.Length == 0)
            {
                MessageBox.Show("Выберите дату рождения");
                return;
            }

            if (Adress_TB.Text.Length == 0)
            {
                MessageBox.Show("Введите адрес");
                return;
            }

            if (Phone_TB.Text.Length == 0)
            {
                MessageBox.Show("Введите телефон");
                return;
            }
            if (Police_TB.Text.Length == 0)
            {
                MessageBox.Show("Введите медицинский полис");
                return;
            }


            selectedPat.FullName = FIO_TB.Text;
            selectedPat.Birthday = Birthday_DP.SelectedDate.Value;
            selectedPat.Adress = Adress_TB.Text;
            selectedPat.Phone = Phone_TB.Text;
            selectedPat.Medical_policy = Police_TB.Text;
            connection.Patient.Add(selectedPat);
            connection.SaveChanges();
            MessageBox.Show("Данные успешно добавлены");
            FIO_TB.Text = "";
            Birthday_DP.Text = null;
            Adress_TB.Text = "";
            Phone_TB.Text = "";
            Police_TB.Text = "";
            Patients.Add(selectedPat);
            List_Patient.SelectedItem = selectedPat;

        }




        private void Look_Btn_Click(object sender, RoutedEventArgs e)
        {

            selectedPat = List_Patient.SelectedItem as Patient;

            if (List_Patient.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите пациента!");
            }
            else
            {
                if (selectedPat != null)
                {

                    Adress_TB.Text = selectedPat.Adress;
                    Phone_TB.Text = selectedPat.Phone;
                    Police_TB.Text = selectedPat.Medical_policy;



                }
            }
        }



        //private void EmployeeChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    selectedPat = List_Patient.SelectedItem as Patient;
        //    Adress_TB.Text = selectedPat.Adress;
        //    Phone_TB.Text = selectedPat.Phone;
        //    Police_TB.Text = selectedPat.Medical_policy;
            
        //}


        private void Edit_Btn_Click(object sender, RoutedEventArgs e)
        {
            selectedPat = List_Patient.SelectedItem as Patient;
            if (List_Patient.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите пациента!");
                return;
            }

            else
            {
                if (selectedPat != null)
                {

                    var pat_edit = connection.Patient.Where(o => o.ID == selectedPat.ID).FirstOrDefault();
                    if (pat_edit != null)
                    {
                        if (Adress_TB.Text.Length == 0)
                        {
                            MessageBox.Show("Введите адрес!");
                            return;
                        }

                        if (Phone_TB.Text.Length == 0)
                        {
                            MessageBox.Show("Введите телефон!");
                            return;
                        }

                        if (Police_TB.Text.Length == 0)
                        {
                            MessageBox.Show("Введите медицинский полис!");
                            return;
                        }

                        pat_edit.Adress = Adress_TB.Text;
                        pat_edit.Phone = Phone_TB.Text;
                        pat_edit.Medical_policy = Police_TB.Text;
                        MessageBox.Show("Изменения сохранены");
                        Adress_TB.Text = "";
                        Phone_TB.Text = "";
                        Police_TB.Text = "";
                        //Patients.Add(selectedPat);

                        //List_Patient.SelectedItem = pat_edit;

                    }
                }
            }
            connection.SaveChanges();
        }

        private void Search_TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchBy_P(Search_TB.Text);
        }

        private void Add_Btn_D_Click(object sender, RoutedEventArgs e)
        {
            selectedDoc = new Doctor();

            if (FIO_TB_D.Text.Length == 0)
            {
                MessageBox.Show("Введите Ф.И.О.");
                return;
            }

            if (Spec_CB.Text.Length == 0)
            {
                MessageBox.Show("Выберите специальность");
                return;
            }

            if (Phone_TB_D.Text.Length == 0)
            {
                MessageBox.Show("Введите телефон");
                return;
            }
           


            selectedDoc.FullName = FIO_TB_D.Text;
            selectedDoc.Specialization = Spec_CB.Text;
            selectedDoc.Phone = Phone_TB_D.Text;
            connection.Doctor.Add(selectedDoc);
            connection.SaveChanges();
            MessageBox.Show("Данные успешно добавлены");
            FIO_TB_D.Text = "";
            Spec_CB.Text = "";
            Phone_TB_D.Text = "";
            Doctors.Add(selectedDoc);
            List_Doctor.SelectedItem = selectedDoc;
        }

        private void Look_Btn_D_Click(object sender, RoutedEventArgs e)
        {
            selectedDoc = List_Doctor.SelectedItem as Doctor;

            if (List_Doctor.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите врача!");
            }
            else
            {
                if (selectedDoc != null)
                {

                    
                    Phone_TB_D.Text = selectedDoc.Phone;
                   



                }
            }
        }

        private void Edit_Btn_D_Click(object sender, RoutedEventArgs e)
        {
            selectedDoc = List_Doctor.SelectedItem as Doctor;
            if (List_Doctor.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите врача!");
                return;
            }

            else
            {
                if (selectedDoc != null)
                {

                    var doc_edit = connection.Doctor.Where(o => o.ID == selectedDoc.ID).FirstOrDefault();
                    if (doc_edit != null)
                    {

                        

                        if (Phone_TB_D.Text.Length == 0)
                        {
                            MessageBox.Show("Введите телефон!");
                            return;
                        }

                       

                        doc_edit.Phone = Phone_TB_D.Text;
                        
                        MessageBox.Show("Изменения сохранены");
                        
                        Phone_TB_D.Text = "";
                        
                        //Patients.Add(selectedPat);

                        List_Doctor.SelectedItem = selectedDoc;

                    }
                }
            }
            connection.SaveChanges();
        }

        private void Search_TB_D_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchBy_D(Search_TB_D.Text);
        }

        private void Filter_Spec_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Specialization specialization = Filter_Spec.SelectedItem as Specialization;
            FilterBySpec(specialization.Name);
            
        }

        private void Sort_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortItem sortItem = Sort_CB.SelectedItem as SortItem;
            SortByName(sortItem.PropertyName, sortItem.Ascending);
        }

        private void Filter_Spec_Pr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Specialization specialization = Filter_Spec_Pr.SelectedItem as Specialization;
            FilterBySpec(specialization.Name);
        }

        private void Search_TB_Pr_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchBy_P(Search_TB_Pr.Text);
        }

        

        private void Add_Btn_Priem_Click(object sender, RoutedEventArgs e)
        {
            selectedRec = new Reception();

            if (List_Doctor_Pr.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите врача!");
                return;
            }

            if (List_Patient_Pr.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите пациента!");
                return;
            }


            if (Date_Priem.Text.Length == 0)
            {
                MessageBox.Show("Выберите дату приёма!");
                return;
            }

           

            else
            {
                Dictionary<int, int> currentOrderList_3 = new Dictionary<int, int>();
                foreach (var Doctor in List_Doctor_Pr.SelectedItems)
                {
                    var emp_C = Doctor as Doctor;
                    if (currentOrderList_3.ContainsKey(emp_C.ID))
                    {
                        currentOrderList_3[emp_C.ID]++;
                    }
                    else
                    {
                        currentOrderList_3.Add(emp_C.ID, 1);
                    }
                }


                Dictionary<int, int> currentOrderList_2 = new Dictionary<int, int>();
                foreach (var Patient in List_Patient_Pr.SelectedItems)
                {
                    var emp_C = Patient as Patient;
                    if (currentOrderList_2.ContainsKey(emp_C.ID))
                    {
                        currentOrderList_2[emp_C.ID]++;
                    }
                    else
                    {
                        currentOrderList_2.Add(emp_C.ID, 1);
                    }
                }

                

                foreach (var Employee1 in currentOrderList_2)
                {
                    var emp_C = Employee1.Value;
                   
                    selectedRec.Patient = Employee1.Key;
                    
                }

                foreach (var Employee in currentOrderList_3)
                {
                    var emp_C = Employee.Value;
                    
                    selectedRec.Doctor = Employee.Key;
                    selectedRec.Date = Date_Priem.SelectedDate;
                   
                }
                connection.Reception.Add(selectedRec);
                Receptions.Add(selectedRec);
                List_Reception.SelectedItem = selectedRec;

                int result = connection.SaveChanges();
                if (result > 0)
                {
                    

                    MessageBox.Show("Данные добавлены");
                    Date_Priem.Text = null;
                }
                else
                {
                    MessageBox.Show("ERROR");
                }

            }
        }

        private void Search_TB_Pr_Doctor_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchBy_Rec(Search_TB_Pr_Doctor.Text);
        }

        private void Del_Btn_Click(object sender, RoutedEventArgs e)
        {
            selectedPat = List_Patient.SelectedItem as Patient;
            if (List_Patient.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите пациента!");
                return;
            }

            else
            {
                if (selectedPat != null)
                {

                   connection.Patient.Remove(selectedPat);
                   Patients.Remove(selectedPat);
                   connection.SaveChanges();
                   MessageBox.Show("Изменения сохранены");
                    

                    
                }
            }
            
        }

        private void Del_Btn_D_Click(object sender, RoutedEventArgs e)
        {
            selectedDoc = List_Doctor.SelectedItem as Doctor;
            if (List_Doctor.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите врача!");
                return;
            }

            else
            {
                if (selectedDoc != null)
                {

                    connection.Doctor.Remove(selectedDoc);
                    Doctors.Remove(selectedDoc);
                    connection.SaveChanges();
                    MessageBox.Show("Изменения сохранены");



                }
            }
        }

        private void Del_Btn_Priem_Click(object sender, RoutedEventArgs e)
        {
            selectedRec = List_Reception.SelectedItem as Reception;
            if (List_Reception.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите приём!");
                return;
            }

            else
            {
                if (selectedRec != null)
                {

                    connection.Reception.Remove(selectedRec);
                    Receptions.Remove(selectedRec);
                    connection.SaveChanges();
                    MessageBox.Show("Изменения сохранены");



                }
            }
        }

        private void Filter_Spec_Tre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Specialization specialization = Filter_Spec_Tre.SelectedItem as Specialization;
            FilterBySpec(specialization.Name);
        }

        private void Search_TB_Tre_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchBy_P(Search_TB_Tre.Text);
        }

        private void Search_TB_Tre_Doctor_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchBy_Tre(Search_TB_Tre_Doctor.Text);
        }

        private void Add_Btn_Tre_Click(object sender, RoutedEventArgs e)
        {
            selectedTre = new Treatment();

            if (List_Doctor_Tre.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите врача!");
                return;
            }

            if (List_Patient_Tre.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите пациента!");
                return;
            }

            if (Date_Tre.Text.Length == 0)
            {
                MessageBox.Show("Выберите дату лечения!");
                return;
            }

            if (Infor_TB.Text.Length == 0)
            {
                MessageBox.Show("Введите сведения о лечении!");
                return;
            }

            else
            {
                Dictionary<int, int> currentOrderList_3 = new Dictionary<int, int>();
                foreach (var Doctor in List_Doctor_Tre.SelectedItems)
                {
                    var emp_C = Doctor as Doctor;
                    if (currentOrderList_3.ContainsKey(emp_C.ID))
                    {
                        currentOrderList_3[emp_C.ID]++;
                    }
                    else
                    {
                        currentOrderList_3.Add(emp_C.ID, 1);
                    }
                }


                Dictionary<int, int> currentOrderList_2 = new Dictionary<int, int>();
                foreach (var Patient in List_Patient_Tre.SelectedItems)
                {
                    var emp_C = Patient as Patient;
                    if (currentOrderList_2.ContainsKey(emp_C.ID))
                    {
                        currentOrderList_2[emp_C.ID]++;
                    }
                    else
                    {
                        currentOrderList_2.Add(emp_C.ID, 1);
                    }
                }



                foreach (var Pat in currentOrderList_2)
                {
                    var emp_C = Pat.Value;

                    selectedTre.Patient = Pat.Key;

                }

                foreach (var Doc in currentOrderList_3)
                {
                    var emp_C = Doc.Value;

                    selectedTre.Doctor = Doc.Key;
                    selectedTre.Date = Date_Tre.SelectedDate;
                    selectedTre.Information = Infor_TB.Text;

                }
                connection.Treatment.Add(selectedTre);
                Treatments.Add(selectedTre);
                List_Treatment.SelectedItem = selectedTre;

                int result = connection.SaveChanges();
                if (result > 0)
                {
                    
                    MessageBox.Show("Данные добавлены");
                    Date_Tre.Text = null;
                    Infor_TB.Text = "";
                }
                else
                {
                    MessageBox.Show("ERROR");
                }

            }
        }

        private void Del_Btn_Tre_Click(object sender, RoutedEventArgs e)
        {
            selectedTre = List_Treatment.SelectedItem as Treatment;
            if (List_Treatment.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите лечение!");
                return;
            }

            else
            {
                if (selectedTre != null)
                {

                    connection.Treatment.Remove(selectedTre);
                    Treatments.Remove(selectedTre);
                    connection.SaveChanges();
                    MessageBox.Show("Изменения сохранены");



                }
            }
        }
    }
}
