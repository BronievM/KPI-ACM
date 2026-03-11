import math

class CalculatorLogic:
    LINEAR_FORMULA = "f = ∛(5 + c·√(b + 5√a))"
    BRANCHING_FORMULA = (
        "k > 10  →  f = √(k·√(d²) + d·√(k²))\n"
        "k ≤ 10 →  f = (k + d)²"
    )
    CYCLIC_FORMULA = ( "f = ∏ᵢ₌₁ⁿ ( aᵢ + bᵢ₊₁) + Σᵢⁿ (aⱼ₊₁·bⱼ)" )

    LINEAR_IMG = "media/linear.png"
    BRANCHING_IMG = "media/branching.png"
    CYCLIC_IMG = "media/cyclic.png"

    def calculate_linear(a, b, c):
        if a < 0:
            raise ValueError("Параметр 'a' повинен бути >= 0")
        term_b = b + 5 * math.sqrt(a)
        if term_b < 0:
            raise ValueError("Вираз (b + 5*sqrt(a)) повинен бути >= 0")
        base = 5 + c * math.sqrt(term_b)
        if base >= 0:
            return base ** (1 / 3)
        else:
            return - (abs(base) ** (1 / 3))

    def calculate_branching(k, d):
        if k > 10:
            val = k * abs(d) + d * k
            if val < 0:
                raise ValueError("Підкореневий вираз від'ємний")
            return math.sqrt(val)
        else:
            return (k + d) ** 2

    def calculate_cyclic(n, a_list, b_list):
        if len(a_list) < n + 1 or len(b_list) < n + 1:
            raise ValueError(f"Для n={n} потрібно {n+1} чисел у масивах!")

        product_part = 1.0
        sum_part = 0.0
        for i in range(1, n + 1):
            product_part *= (a_list[i - 1] + b_list[i])
            sum_part += (a_list[i] * b_list[i - 1])

        return product_part + sum_part