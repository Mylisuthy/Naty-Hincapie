<script setup lang="ts">
import { ref } from 'vue';

const email = ref('');
const password = ref('');

const handleLogin = () => {
  console.log('Login attempt', email.value);
};
</script>

<template>
  <div class="win11-window">
    <div class="logo-area">
      <h1 class="logo-text">Naty</h1>
      <p class="welcome-text">Bienvenido a Naty</p>
    </div>

    <form @submit.prevent="handleLogin">
      <div class="input-group">
        <input 
          v-model="email" 
          type="email" 
          placeholder="Correo electrónico" 
          class="fluent-input" 
          required
        />
      </div>
      
      <div class="input-group">
        <input 
          v-model="password" 
          type="password" 
          placeholder="Contraseña" 
          class="fluent-input" 
          required
        />
      </div>

      <button type="submit" class="fluent-button">
        Iniciar Sesión
      </button>

      <div class="links">
        <a href="#" class="sub-link">¿No tienes cuenta? Regístrate</a>
      </div>
    </form>
  </div>
</template>

<style scoped>
.logo-area {
  margin-bottom: 30px;
}

.welcome-text {
  font-size: 14px;
  color: var(--text-secondary);
  margin-top: -5px;
}

.links {
  margin-top: 20px;
  font-size: 13px;
}

.sub-link {
  color: var(--accent-color);
  text-decoration: none;
}

.sub-link:hover {
  text-decoration: underline;
}
</style>
