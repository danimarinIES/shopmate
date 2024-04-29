package com.example.shopmate_app

import android.annotation.SuppressLint
import android.os.Bundle
import android.widget.Toast
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.navigation.NavController
import androidx.navigation.findNavController
import com.example.shopmate_app.databinding.ActivityMainBinding

class MainActivity : AppCompatActivity() {
    private var shouldGoBack : Boolean = false

    private lateinit var navController: NavController

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_main)
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



    @SuppressLint("MissingSuperCall")
    override fun onBackPressed() {
        // shouldGoBack is defined in every activity, to do a custom action with the back button
        // if this variable is set to true, the user can go back normally, if not then we can
        // do a custom action.
        if (shouldGoBack) {
            super.onBackPressed()
        } else {
            Toast.makeText(applicationContext, "Back Button Pressed", Toast.LENGTH_SHORT).show()
        }

    }
}