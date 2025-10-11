import subprocess
import time

# --- RUTAS ABSOLUTAS ---
BACKEND_PATH = r"C:\Kevin\Cosas\TAREAS\Tareas 2025 SEG\Web2\Git\REACT\encuestasapp\backend"
FRONTEND_PATH = r"C:\Kevin\Cosas\TAREAS\Tareas 2025 SEG\Web2\Git\REACT\encuestasapp\front"

# --- COMANDOS ---
CMD_BACKEND = ["dotnet", "run"]
CMD_FRONTEND = ["npm", "start"]

def run_backend():
    print("🚀 Iniciando backend (.NET)...")
    return subprocess.Popen(CMD_BACKEND, cwd=BACKEND_PATH, shell=True)

def run_frontend():
    print("🌐 Iniciando frontend (React)...")
    return subprocess.Popen(CMD_FRONTEND, cwd=FRONTEND_PATH, shell=True)

if __name__ == "__main__":
    try:
        # Iniciar backend
        backend_process = run_backend()

        # Esperar unos segundos para que el backend arranque antes del frontend
        time.sleep(5)

        # Iniciar frontend
        frontend_process = run_frontend()

        print("\n✅ Proyecto iniciado correctamente!")
        print("➡ Backend corriendo en http://localhost:5204")
        print("➡ Frontend corriendo en http://localhost:3000\n")

        # Mantener ambos procesos activos
        backend_process.wait()
        frontend_process.wait()

    except KeyboardInterrupt:
        print("\n🛑 Cerrando ambos procesos...")
        backend_process.terminate()
        frontend_process.terminate()
        print("Procesos finalizados correctamente.")
