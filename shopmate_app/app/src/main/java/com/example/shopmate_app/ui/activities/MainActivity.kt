package com.example.shopmate_app.ui.activities

import android.os.Bundle
import android.util.Log
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.navigation.NavController
import androidx.navigation.findNavController
import com.example.shopmate_app.R
import com.example.shopmate_app.databinding.ActivityMainBinding
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class MainActivity : AppCompatActivity() {
    private lateinit var binding: ActivityMainBinding
    private lateinit var navController: NavController

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        Log.e("ENTROOOOOOOOOOOOOOO", "OLAAAAAAAAAAAAAAAA")
        Log.e("ENTROOOOOOOOOOOOOOO", "OLAAAAAAAAAAAAAAAA")
        Log.e("ENTROOOOOOOOOOOOOOO", "OLAAAAAAAAAAAAAAAA")
        Log.e("ENTROOOOOOOOOOOOOOO", "OLAAAAAAAAAAAAAAAA")
        Log.e("ENTROOOOOOOOOOOOOOO", "OLAAAAAAAAAAAAAAAA")
        Log.e("ENTROOOOOOOOOOOOOOO", "OLAAAAAAAAAAAAAAAA")
        setContentView(binding.root)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
    }

    override fun onSupportNavigateUp(): Boolean {
        navController = findNavController(R.id.navHostMainFragmentContainer)
        return navController.navigateUp() || super.onSupportNavigateUp()
    }
}