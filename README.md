# Lab_04_WPF

Treść zadania
___________________________________________________________
Programowanie w Środowisku Windows LAB 3 – WinForms + MySQL
Liczba punktów do zdobycia: 10pkt + 5pkt

Wstęp: Formularz logowania i rejestracji na wydarzenie – aplikacja okienkowa Przebieg ćwiczenia – kontynuacja poprzedniego ćwiczenia:
1.	Po zalogowaniu jako user przechodzimy do widoku zapisu na wydarzenie:
  a.	Pola:
    i.	Nazwa wydarzenie – pobierana do comboboxa z bazy danych
    ii.	Agenda – wczytana z bazy danych po wyborze nazwy wydarzenia
    iii.	Termin wydarzenia
    iv.	Typ uczestnictwa:
      1.	Słuchacz
      2.	Autor
      3.	Sponsor
      4.	Organizator
    v.	Wyżywienie
      1.	Bez preferencji
      2.	Wegetariańskie
      3.	Bez glutenu
  b.	Zgłoszenie na wydarzenie przypisane jest do zalogowanego użytkownika
2.	Po zalogowaniu jako administrator przechodzimy do panelu administratora:
  a.	Obsługa użytkowników
    i.	Dodawanie
    ii.	Usuwanie
    iii.	Reset hasła
  b.	Obsługa zapisów na wydarzenie
    i.	Potwierdzenie zapisu
    ii.	Odrzucenie zapisu
___________________________________________________________


Modyfikacje
___________________________________________________________
1. Za przyzwoleniem prowadzącego wykorzystałam WPF zamiast WinForms, aby sobie nieco "podnieść poprzeczkę", jeśli można to tym nazwać.
2. Na dodatkowe punkty należało zaimplementować wysyłanie automatycznych maili.
3. Chcąc zwiększyć bezpieczeństwo programu do zakodowania haseł przechowywanych w bazie danych wykorzystałam szyfr kryptograficzny.
