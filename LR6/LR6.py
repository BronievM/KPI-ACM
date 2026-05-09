import os

def run_turing_machine(tape_str, transitions, start_state='q0', halt_state='qs', blank='_', show_trace=True):
    tape = list(tape_str)
    head = 0
    state = start_state
    step = 1

    if show_trace:
        print(f"{'Крок':<5} | {'Стрічка':<25} | {'Стан':<5} | {'Зчитано':<8} | {'Записано':<9} | {'Рух':<4} | {'Наст. стан'}")
        print("-" * 85)

    while state != halt_state:
        if head >= len(tape):
            tape.append(blank)
        elif head < 0:
            tape.insert(0, blank)
            head = 0

        symbol = tape[head]
        tape_display = "".join([f"[{c}]" if i == head else c for i, c in enumerate(tape)])

        if (state, symbol) not in transitions:
            if show_trace:
                print(f"{step:<5} | {tape_display:<25} | {state:<5} | {symbol:<8} | Помилка")
            break

        new_state, new_symbol, direction = transitions[(state, symbol)]

        if show_trace:
            print(
                f"{step:<5} | {tape_display:<25} | {state:<5} | {symbol:<8} | {new_symbol:<9} | {direction:<4} | {new_state}")

        tape[head] = new_symbol
        state = new_state

        if direction == 'R':
            head += 1
        elif direction == 'L':
            head -= 1

        step += 1

    return "".join(tape).replace(blank, '')

transitions = {
    ('q0', '1'): ('q0', '1', 'R'), # Пропуск одиниць першого числа
    ('q0', '0'): ('q1', '1', 'R'), # Заміна '0' на '1'
    ('q1', '+'): ('q2', '1', 'R'), # Заміна '+' на '1'
    ('q2', '1'): ('q2', '1', 'R'), # Пропуск одиниць другого числа
    ('q2', '0'): ('q3', '_', 'L'), # Стирання кінцевого '0', крок вліво
    ('q3', '1'): ('q4', '_', 'L'), # Стирання зайвої '1', крок вліво
    ('q4', '1'): ('qs', '0', 'E')  # Заміна передостанньої '1' на '0', завершення (qs)
}

def validate_input(data):
    if not all(c in '01+' for c in data):
        return False
    if data.count('+') != 1 or data.count('0') != 2:
        return False
    parts = data.split('+')
    if len(parts) != 2 or parts[0].count('0') != 1 or parts[1].count('0') != 1:
        return False
    return True

def process_data(data):
    if validate_input(data):
        print(f"\nПочаткова стрічка: {data}\n")
        result = run_turing_machine(data, transitions)
        print(f"\nРезультат: {result}\n")
    else:
        print("Помилка: Неправильний формат. Очікується 'X+Y', де X та Y - унарні числа (наприклад, 1110+110).\n")


def run_tests():
    test_cases = [
        ("1110+110", "111110"),  # 3 + 2 = 5
        ("10+11110", "111110"),  # 1 + 4 = 5
        ("11110+1110", "11111110"),  # 4 + 3 = 7
        ("10+10", "110"),  # 1 + 1 = 2
        ("111110+10", "1111110")  # 5 + 1 = 6
    ]

    print("\n--- Запуск автоматичних тестів ---")
    passed = 0
    for input_data, expected in test_cases:
        result = run_turing_machine(input_data, transitions, start_state='q0', halt_state='qs', show_trace=False)
        status = "УСПІШНО" if result == expected else "ПОМИЛКА"
        if status == "УСПІШНО":
            passed += 1
        print(f"Вхід: {input_data:<12} | Очікується: {expected:<10} | Результат: {result:<10} | {status}")

    print(f"Пройдено тестів: {passed}/{len(test_cases)}\n")

def main():
    while True:
        print("1. Введення з клавіатури")
        print("2. Зчитати з файлу")
        print("3. Запустити автоматичні тести")
        print("4. Вихід")
        choice = input("Вибір: ")

        if choice == '1':
            data = input("Введіть стрічку (наприклад, 1110+110): ").strip()
            process_data(data)
        elif choice == '2':
            filename = input("Ім'я файлу (наприклад, input.txt): ").strip()
            if os.path.exists(filename):
                with open(filename, 'r', encoding='utf-8') as f:
                    data = f.read().strip()
                process_data(data)
            else:
                print("Файл не знайдено.\n")
        elif choice == '3':
            run_tests()
        elif choice == '4':
            break
        else:
            print("Невірний вибір.\n")

if __name__ == "__main__":
    main()