﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using ShopMate_Client_V1.Model;
using ShopMate_Client_V1.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopMate_Client_V1.Controller
{
  
    public class ControllerCategory
    {
        Category cAux;
        Item iAux;
        FormCategory fCatForm;
        FormItem fItemForm;
        Repository r;
        DataGridView dtgCat;
        DataGridView dtgItem;
        List<Category> categoryList;
        List<Item> itemList;
        UserImageAux imageAux = new UserImageAux();
        bool hasItems;


        public ControllerCategory(Repository r, DataGridView dtg_cat, DataGridView dtg_item) {

             cAux = new Category();
            iAux = new Item();
             fCatForm = new FormCategory();
            fItemForm = new FormItem();
             dtgCat = dtg_cat;
             dtgItem = dtg_item;
             this.r = r;
             categoryList = r.GetCategories();
            itemList = r.GetItems();

            LoadData();
            InitListeners();

        }

        private void LoadData()
        {
            dtgCat.DataSource = r.GetCategories(); 
            dtgItem.DataSource = itemList;   
        }

        public void InitListeners()
        {
            // CATEGORY FORM-----------------------------
            fCatForm.btn_add_image_cat.Click += addImage;
            dtgCat.CellDoubleClick += modifyCategory;
            dtgCat.SelectionChanged += itemsFromCategory;
            fCatForm.btn_addCat.Click += postCategory;
            fCatForm.btn_back_fCat.Click += goBack;
            // ITEM FORM-----------------------------
            fItemForm.btn_add.Click += postItem;
            dtgItem.CellDoubleClick += modifyItem; 



        }

       
        private void itemsFromCategory(object sender, EventArgs e)
        {
           
            
           List<Item> itemList = r.GetItems().Where(i => i.CategoryId == selectedDGV_Category().CategoryId).ToList();
           int nItems =  r.GetItems().Where(i => i.CategoryId == selectedDGV_Category().CategoryId).Count();
           dtgItem.DataSource = itemList;
            if (nItems > 0)            {
                hasItems = true;
            } else
            {
                hasItems = false;
            }

        }

        internal void openFormCategory(object sender, EventArgs e)
        {
            
                Button btnAux = sender as Button;
            

                if (btnAux.Text.Equals("Add"))
                {

                fCatForm.btn_addCat.Text = "Add category";
                fCatForm.ShowDialog();
                   
                }

         }

        private Category selectedDGV_Category()
        {
            int rowIndex = dtgCat.CurrentCell.RowIndex;

            if (rowIndex >= 0 && rowIndex < dtgCat.Rows.Count)
            {
                DataGridViewRow selectedRow = dtgCat.Rows[rowIndex];
                Category selectedCategory = selectedRow.DataBoundItem as Category;
              
                return selectedCategory;
            }

           

            return null;
        }       

        private void modifyCategory(object sender, DataGridViewCellEventArgs e)
        {
            fCatForm.btn_addCat.Text = "Modify category";

            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dtgCat.Rows[e.RowIndex];
                Category categorySelected = selectedRow.DataBoundItem as Category;
                String newName = fCatForm.txt_name.Text.ToString();

                if (categorySelected != null)
                {
                    fCatForm.txt_name.Text = categorySelected.Name.ToString();

                    Image categoryImage = r.GetImageLocal(categorySelected.Icon.ToString());
                    fCatForm.pictureBox_cat.SizeMode = PictureBoxSizeMode.Zoom;
                    fCatForm.pictureBox_cat.Image = categoryImage;

                    cAux = categorySelected;

                }

            }
            fCatForm.ShowDialog();

        }

        private void addImage(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos de imagen (*.jpg, *.png)|*.jpg;*.png";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                Bitmap originalImage = (Bitmap)Image.FromFile(fileName);
                Bitmap resizedImage = new Bitmap(originalImage, new Size(fCatForm.pictureBox_cat.Width, fCatForm.pictureBox_cat.Height));
                fCatForm.pictureBox_cat.Image = resizedImage;
              
            }
        }

        private async void postCategory(object sender, EventArgs e)
        {
            if (fCatForm.btn_addCat.Text.Equals("Add category"))
            {
                cAux.Name = fCatForm.txt_name.Text.ToString();
                cAux.UpdatedAt = DateTime.Now;
                cAux.CreatedAt = DateTime.Now;

                
                Image categoryImage = fCatForm.pictureBox_cat.Image;
                if (categoryImage != null)
                {
                    try
                    {
                        string imageUrl = await r.PostImageAsync("category", categoryImage);
                        cAux.Icon = imageUrl;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error uploading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    cAux.Icon = "";
                }

                r.PostCategory(cAux);
                goBack(sender, e);
            }

            if (fCatForm.btn_addCat.Text.Equals("Modify category"))
            {
                String newName = fCatForm.txt_name.Text.ToString();
                uint categoryId = selectedDGV_Category().CategoryId;

                // Subir la imagen si existe
                //Image categoryImage = fCatForm.pictureBox_cat.Image;
                //if (categoryImage != null)
                //{
                //    try
                //    {
                //        // string imageUrl = await r.PostImageAsync("category", categoryImage);
                //        // selectedDGV_Category().Icon = imageUrl;
                //    }
                //    catch (Exception ex)
                //    {
                //        MessageBox.Show($"Error uploading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        return;
                //    }
                //}

                r.PutCategory(cAux, categoryId, newName);
                goBack(sender, e);
            }

            LoadData();
        }

        private void goBack(object sender, EventArgs e)
        {
            fCatForm.Close();
        }      

        internal void categoryFilter(string v)
        {
            if (String.IsNullOrEmpty(v))
            {
                LoadData();
               
            }
            //List<UserDTO> filteredUserDTOList = userDTOList.Where(u => u.Name.ToLower().Contains(actualText.ToLower())).ToList();

            //f.dtg_client.DataSource = filteredUserDTOList;
            List<Category> filteredCategoriesList = categoryList.Where(c => c.Name.ToLower().Contains(v.ToLower())).ToList();
            // categoryList = categoryList.Where(c => c.Name.ToLower().Contains(v.ToLower())).ToList();
            dtgCat.DataSource = filteredCategoriesList;
        }

        internal void orderByComboCategory(string selectedOrder)
        {
            if(!String.IsNullOrEmpty(selectedOrder))
            {
                try
                {

                    switch (selectedOrder)
                    {
                        case "Name":

                            categoryList = categoryList.OrderBy(c => c.Name).ToList();
                            break;

                        case "Last Update":
                            categoryList = categoryList.OrderBy(c => c.UpdatedAt).ToList();
                            break;

                        case "Register Date":

                            categoryList = categoryList.OrderBy(c => c.CreatedAt).ToList();
                            break;
                        case " ":
                            categoryList = r.GetCategories();
                            break;
                           
                    }
                    dtgCat.DataSource = categoryList;
                }
             
                 catch
                        {
                    MessageBox.Show("CAN'T CONNECT TO THE API", "Empty fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
             
            }
            
        }

        internal void openFormItem(object sender, EventArgs e)
        {
           fItemForm.combo_category.DataSource = r.GetCategories().Select(c => c.Name).ToList();
           fItemForm.ShowDialog(); 
        }

        internal void orderByComboItem(string selectedOrder)
        {
            if (!String.IsNullOrEmpty(selectedOrder))
            {
                try
                {

                    switch (selectedOrder)
                    {
                        case "Name":

                            itemList = itemList.OrderBy(c => c.Name).ToList();
                            break;

                        case "Last Update":
                            itemList = itemList.OrderBy(c => c.UpdatedAt).ToList();
                            break;

                        case "Register Date":

                            itemList = itemList.OrderBy(c => c.CreatedAt).ToList();
                            break;

                        case "Category":
                            itemList = itemList.OrderBy(C => C.CategoryId).ToList();
                            break;
                        case " ":
                            itemList = r.GetItems();
                            break;

                    }
                    dtgItem.DataSource = itemList;
                }

                catch
                {
                    MessageBox.Show("CAN'T CONNECT TO THE API", "Empty fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
        }

        private async void postItem(object sender, EventArgs e)
        {
            String selectedCategoryName = fItemForm.combo_category.Text.ToString();
            Category selectedCategory = categoryList.FirstOrDefault(c => c.Name.Equals(selectedCategoryName));

            if (selectedCategory == null)
            {
                MessageBox.Show("The selected category does not exist. Please select a valid category.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            iAux.Name = fItemForm.txt_name.Text.ToString();
            iAux.CategoryId = selectedCategory.CategoryId;
            iAux.UpdatedAt = DateTime.Now;

            if (fItemForm.btn_add.Text.Equals("Add"))
            {
                iAux.CreatedAt = DateTime.Now;
                r.PostItem(iAux);
            }
            else if (fItemForm.btn_add.Text.Equals("Modify"))
            {
                r.PutItem(iAux, iAux.ItemId, iAux.Name, (uint)iAux.CategoryId);
            }

            fItemForm.Close();
            itemList = r.GetItems(); // Asegúrate de recargar la lista de ítems después de la operación
            dtgItem.DataSource = itemList;
        }

        private void modifyItem(object sender, DataGridViewCellEventArgs e)
        {
            fItemForm.btn_add.Text = "Modify Item"; 

            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dtgItem.Rows[e.RowIndex];
                Item selectedItem = selectedRow.DataBoundItem as Item;

                if (selectedItem != null)
                {
                    iAux = selectedItem;
                    fItemForm.txt_name.Text = selectedItem.Name.ToString();
                    fItemForm.combo_category.SelectedItem = categoryList.FirstOrDefault(c => c.CategoryId == selectedItem.CategoryId)?.Name;

                    fItemForm.ShowDialog();
                }
            }
        }
        private Item selectedDGV_Item()
        {
            int rowIndex = dtgItem.CurrentCell.RowIndex;

            if (rowIndex >= 0 && rowIndex < dtgItem.Rows.Count)
            {
                DataGridViewRow selectedRow = dtgItem.Rows[rowIndex];
                return selectedRow.DataBoundItem as Item;
            }

            return null;
        }
        public void defaultDGV_items()
        {
            dtgItem.DataSource = r.GetItems();
        }

       
    }
}

