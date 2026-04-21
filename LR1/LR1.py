import tkinter as tk
from tkinter import ttk, messagebox, filedialog
from PIL import Image, ImageTk
import random
import os
from MathLogic import CalculatorLogic
import utils

class MainWindow(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("ЛР1")

        menubar = tk.Menu(self)
        file_menu = tk.Menu(menubar, tearoff=0)
        file_menu.add_command(label="Зберегти", command=self.save_file)
        file_menu.add_command(label="Відкрити", command=self.load_file)
        menubar.add_cascade(label="Файл", menu=file_menu)
        self.config(menu=menubar)

        self.notebook = ttk.Notebook(self)
        self.notebook.pack(expand=True, fill="both", padx=10, pady=10)

        self.tab1 = ttk.Frame(self.notebook)
        self.tab2 = ttk.Frame(self.notebook)
        self.tab3 = ttk.Frame(self.notebook)

        self.notebook.add(self.tab1, text="Лінійний")
        self.notebook.add(self.tab2, text="Розгалужений")
        self.notebook.add(self.tab3, text="Циклічний")

        self.setup_tab1()
        self.setup_tab2()
        self.setup_tab3()

    def show_image(self, parent, path, text):
        try:
            if not os.path.exists(path):
                raise Exception

            img = Image.open(path)
            w_base = 450
            ratio = (w_base / float(img.size[0]))
            h_new = int((float(img.size[1]) * float(ratio)))
            img = img.resize((w_base, h_new), Image.Resampling.LANCZOS)
            photo = ImageTk.PhotoImage(img)

            lbl = tk.Label(parent, image=photo)
            lbl.image = photo
            lbl.pack(pady=10)
        except:
            lbl = tk.Label(parent, text=text, font=("Cambria Math", 11), pady=15)
            lbl.pack()

    def setup_tab1(self):
        self.show_image(self.tab1, CalculatorLogic.LINEAR_IMG, CalculatorLogic.LINEAR_FORMULA)

        frame = tk.LabelFrame(self.tab1, text="Вхідні дані")
        frame.pack(padx=20, pady=5, fill="x")

        tk.Label(frame, text="a = ").grid(row=0, column=0, padx=5, pady=5)
        self.entry_a = tk.Entry(frame)
        self.entry_a.grid(row=0, column=1, padx=5, pady=5)

        tk.Label(frame, text="b = ").grid(row=1, column=0, padx=5, pady=5)
        self.entry_b = tk.Entry(frame)
        self.entry_b.grid(row=1, column=1, padx=5, pady=5)

        tk.Label(frame, text="c = ").grid(row=2, column=0, padx=5, pady=5)
        self.entry_c = tk.Entry(frame)
        self.entry_c.grid(row=2, column=1, padx=5, pady=5)

        tk.Button(self.tab1, text="Обчислити", command=self.calc1).pack(pady=5)
        self.res1 = tk.Label(self.tab1, text="Результат: -")
        self.res1.pack(pady=10)

    def calc1(self):
        try:
            a = float(self.entry_a.get())
            b = float(self.entry_b.get())
            c = float(self.entry_c.get())
            res = CalculatorLogic.calculate_linear(a, b, c)
            self.res1.config(text="Результат: " + str(round(res, 2)), fg="green")
        except Exception as e:
            self.res1.config(text="Помилка", fg="red")
            messagebox.showerror("Помилка", str(e))

    def setup_tab2(self):
        self.show_image(self.tab2, CalculatorLogic.BRANCHING_IMG, CalculatorLogic.BRANCHING_FORMULA)

        frame = tk.LabelFrame(self.tab2, text="Вхідні дані")
        frame.pack(padx=20, pady=5, fill="x")

        tk.Label(frame, text="k = ").grid(row=0, column=0, padx=5, pady=5)
        self.entry_k = tk.Entry(frame)
        self.entry_k.grid(row=0, column=1, padx=5, pady=5)

        tk.Label(frame, text="d = ").grid(row=1, column=0, padx=5, pady=5)
        self.entry_d = tk.Entry(frame)
        self.entry_d.grid(row=1, column=1, padx=5, pady=5)

        tk.Button(self.tab2, text="Обчислити", command=self.calc2).pack(pady=5)
        self.res2 = tk.Label(self.tab2, text="Результат: -")
        self.res2.pack(pady=10)

    def calc2(self):
        try:
            k = float(self.entry_k.get())
            d = float(self.entry_d.get())
            res = CalculatorLogic.calculate_branching(k, d)
            self.res2.config(text="Результат: " + str(round(res, 2)), fg="green")
        except Exception as e:
            self.res2.config(text="Помилка", fg="red")
            messagebox.showerror("Помилка", str(e))

    def setup_tab3(self):
        self.show_image(self.tab3, CalculatorLogic.CYCLIC_IMG, CalculatorLogic.CYCLIC_FORMULA)

        frame = tk.LabelFrame(self.tab3, text="Вхідні дані")
        frame.pack(padx=20, pady=5, fill="x")

        tk.Label(frame, text="n = ").grid(row=0, column=0, padx=5, pady=5)
        self.entry_n = tk.Entry(frame, width=10)
        self.entry_n.grid(row=0, column=1, padx=5, pady=5, sticky="w")

        tk.Button(frame, text="Заповнити випадковими значеннями", command=self.do_random).grid(row=0, column=2, padx=10)

        tk.Label(frame, text="A:").grid(row=1, column=0, padx=5, pady=5)
        self.text_a = tk.Text(frame, height=3, width=40)
        self.text_a.grid(row=1, column=1, columnspan=2, padx=5, pady=5)

        tk.Label(frame, text="B:").grid(row=2, column=0, padx=5, pady=5)
        self.text_b = tk.Text(frame, height=3, width=40)
        self.text_b.grid(row=2, column=1, columnspan=2, padx=5, pady=5)

        tk.Button(self.tab3, text="Обчислити", command=self.calc3).pack(pady=5)
        self.res3 = tk.Label(self.tab3, text="Результат: -")
        self.res3.pack(pady=5)

    def do_random(self):
        try:
            n = int(self.entry_n.get())
            if n <= 0: return

            arr1 = []
            arr2 = []
            for i in range(n+1):
                arr1.append(random.randint(1, 20))
                arr2.append(random.randint(1, 20))

            self.text_a.delete("1.0", tk.END)
            self.text_b.delete("1.0", tk.END)

            s1 = ""
            for x in arr1:
                s1 += str(x) + ", "

            s2 = ""
            for x in arr2:
                s2 += str(x) + ", "

            self.text_a.insert("1.0", s1[:-2])
            self.text_b.insert("1.0", s2[:-2])
        except:
            messagebox.showwarning("Увага", "Введіть n")

    def calc3(self):
        try:
            n = int(self.entry_n.get())
            str_a = self.text_a.get("1.0", tk.END)
            str_b = self.text_b.get("1.0", tk.END)

            list_a = []
            for x in str_a.replace("\n", " ").split(","):
                if x.strip() != "":
                    list_a.append(float(x))

            list_b = []
            for x in str_b.replace("\n", " ").split(","):
                if x.strip() != "":
                    list_b.append(float(x))

            res = CalculatorLogic.calculate_cyclic(n, list_a, list_b)
            self.res3.config(text="Результат: " + str(round(res, 2)), fg="green")
        except Exception as e:
            self.res3.config(text="Помилка", fg="red")
            messagebox.showerror("Помилка", str(e))

    def load_file(self):
        idx = self.notebook.index(self.notebook.select())
        filename = filedialog.askopenfilename(filetypes=[("Text", "*.txt")])
        if not filename:
            return

        try:
            with open(filename, "r") as f:
                lines = f.readlines()

            clean_lines = []
            for l in lines:
                if l.strip() != "":
                    clean_lines.append(l.strip())

            if idx == 0:
                if len(clean_lines) < 3: return
                self.entry_a.delete(0, tk.END)
                self.entry_a.insert(0, clean_lines[0])
                self.entry_b.delete(0, tk.END)
                self.entry_b.insert(0, clean_lines[1])
                self.entry_c.delete(0, tk.END)
                self.entry_c.insert(0, clean_lines[2])

            elif idx == 1:
                if len(clean_lines) < 2: return
                self.entry_k.delete(0, tk.END)
                self.entry_k.insert(0, clean_lines[0])
                self.entry_d.delete(0, tk.END)
                self.entry_d.insert(0, clean_lines[1])

            elif idx == 2:
                if len(clean_lines) < 3: return
                self.entry_n.delete(0, tk.END)
                self.entry_n.insert(0, clean_lines[0])
                self.text_a.delete("1.0", tk.END)
                self.text_a.insert("1.0", clean_lines[1])
                self.text_b.delete("1.0", tk.END)
                self.text_b.insert("1.0", clean_lines[2])

            messagebox.showinfo("Ок", "Завантажено")
        except:
            messagebox.showerror("Помилка", "Не вдалося відкрити файл")

    def save_file(self):
        idx = self.notebook.index(self.notebook.select())
        filename = filedialog.asksaveasfilename(defaultextension=".txt",
                                                filetypes=[("Text", "*.txt")])
        if not filename:
            return

        try:
            with open(filename, "w") as f:
                if idx == 0:
                    f.write(self.entry_a.get() + "\n")
                    f.write(self.entry_b.get() + "\n")
                    f.write(self.entry_c.get() + "\n")

                elif idx == 1:
                    f.write(self.entry_k.get() + "\n")
                    f.write(self.entry_d.get() + "\n")

                elif idx == 2:
                    f.write(self.entry_n.get() + "\n")
                    f.write(self.text_a.get("1.0", tk.END).strip() + "\n")
                    f.write(self.text_b.get("1.0", tk.END).strip() + "\n")

            messagebox.showinfo("Ок", "Файл збережено")
        except:
            messagebox.showerror("Помилка", "Не вдалося зберегти файл")

if __name__ == "__main__":
    app = MainWindow()
    app.mainloop()