/* ===== Global Styles ===== */
:root {
    --primary - color: #0d6efd;
    --secondary - color: #6c757d;
    --success - color: #198754;
    --danger - color: #dc3545;
    --warning - color: #ffc107;
    --info - color: #0dcaf0;
    --light - color: #f8f9fa;
    --dark - color: #212529;
}

* {
    margin: 0;
    padding: 0;
    box- sizing: border - box;
}

html {
    font - size: 14px;
    scroll - behavior: smooth;
}

body {
    font - family: 'Segoe UI', Tahoma, Geneva, Verdana, sans - serif;
    line - height: 1.6;
    color: var(--dark - color);
    background - color: #f5f5f5;
    min - height: 100vh;
    display: flex;
    flex - direction: column;
}

main {
    flex: 1;
}

/* ===== Typography ===== */
h1, h2, h3, h4, h5, h6 {
    font - weight: 600;
    margin - bottom: 1rem;
}

a {
    color: var(--primary - color);
    text - decoration: none;
    transition: all 0.3s ease;
}

a:hover {
    color: #0b5ed7;
}

/* ===== Navigation ===== */
.navbar {
    z - index: 1030;
}

.navbar - brand {
    font - size: 1.5rem;
    font - weight: bold;
}

.nav - link {
    font - weight: 500;
    transition: all 0.3s ease;
}

.nav - link:hover {
    color: var(--primary - color)!important;
    transform: translateY(-2px);
}

.dropdown - menu {
    box - shadow: 0 0.5rem 1rem rgba(0, 0, 0, .15);
    border: none;
}

/* ===== Loading Overlay ===== */
.loading - overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100 %;
    height: 100 %;
    background: rgba(0, 0, 0, 0.7);
    display: flex;
    justify - content: center;
    align - items: center;
    z - index: 9999;
}

.loading - content {
    text - align: center;
}

/* ===== Product Cards ===== */
.product - card {
    transition: all 0.3s ease;
    border: 1px solid #e0e0e0;
    border - radius: 8px;
    overflow: hidden;
}

.product - card:hover {
    transform: translateY(-5px);
    box - shadow: 0 10px 20px rgba(0, 0, 0, 0.1);
}

.product - card img {
    transition: transform 0.3s ease;
}

.product - card:hover img {
    transform: scale(1.05);
}

.hover - bg - light:hover {
    background - color: #f8f9fa;
}

/* ===== Notifications ===== */
.notification - dropdown {
    box - shadow: 0 0.5rem 1rem rgba(0, 0, 0, .15);
}

.notification - item {
    padding: 10px 15px;
    border - bottom: 1px solid #e9ecef;
    transition: background - color 0.2s ease;
}

.notification - item:hover {
    background - color: #f8f9fa;
}

.notification - item.unread {
    background - color: #e7f3ff;
}

.notification - icon {
    width: 40px;
    height: 40px;
    display: flex;
    align - items: center;
    justify - content: center;
    border - radius: 50 %;
    background - color: #e9ecef;
}

.notification - title {
    font - weight: 600;
    margin - bottom: 5px;
}

.notification - message {
    line - height: 1.4;
}

/* ===== Buttons ===== */
.btn {
    border - radius: 6px;
    padding: 0.5rem 1rem;
    font - weight: 500;
    transition: all 0.3s ease;
}

.btn:hover {
    transform: translateY(-2px);
    box - shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.btn - primary {
    background - color: var(--primary - color);
    border - color: var(--primary - color);
}

.btn - primary:hover {
    background - color: #0b5ed7;
    border - color: #0b5ed7;
}

/* ===== Forms ===== */
.form - control, .form - select {
    border - radius: 6px;
    border: 1px solid #ced4da;
    padding: 0.6rem 1rem;
    transition: all 0.3s ease;
}

.form - control: focus, .form - select:focus {
    border - color: var(--primary - color);
    box - shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.25);
}

.is - invalid {
    border - color: var(--danger - color);
}

.is - valid {
    border - color: var(--success - color);
}

/* ===== Cards ===== */
.card {
    border: 1px solid #e0e0e0;
    border - radius: 8px;
    box - shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
    transition: all 0.3s ease;
}

.card:hover {
    box - shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.card - header {
    background - color: #f8f9fa;
    border - bottom: 1px solid #e0e0e0;
    font - weight: 600;
}

/* ===== Badges ===== */
.badge {
    padding: 0.4em 0.6em;
    font - weight: 500;
    border - radius: 4px;
}

/* ===== Pagination ===== */
.pagination {
    margin - top: 2rem;
}

.page - link {
    color: var(--primary - color);
    border - radius: 6px;
    margin: 0 2px;
    transition: all 0.3s ease;
}

.page - link:hover {
    background - color: var(--primary - color);
    color: white;
    transform: translateY(-2px);
}

.page - item.active.page - link {
    background - color: var(--primary - color);
    border - color: var(--primary - color);
}

/* ===== Footer ===== */
.footer {
    background - color: var(--dark - color);
    margin - top: auto;
}

.footer a {
    color: #adb5bd;
    transition: color 0.3s ease;
}

.footer a:hover {
    color: white;
}

.social - links a {
    display: inline - block;
    transition: transform 0.3s ease;
}

.social - links a:hover {
    transform: translateY(-3px);
}

/* ===== Animations ===== */
@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.fade -in {
    animation: fadeIn 0.5s ease-in -out;
}

@keyframes slideIn {
    from {
        transform: translateX(-100 %);
    }
    to {
        transform: translateX(0);
    }
}

.slide -in {
    animation: slideIn 0.5s ease-in -out;
}

@keyframes pulse {
    0 %, 100 % {
        transform: scale(1);
    }
    50 % {
        transform: scale(1.05);
    }
}

.price - update - animation {
    animation: pulse 0.5s ease -in -out;
    color: var(--success - color)!important;
}

/* ===== Utilities ===== */
.shadow - sm - hover:hover {
    box - shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075)!important;
}

.shadow - hover:hover {
    box - shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15)!important;
}

.shadow - lg - hover:hover {
    box - shadow: 0 1rem 3rem rgba(0, 0, 0, 0.175)!important;
}

/* ===== Responsive ===== */
@media(max - width: 768px) {
    html {
        font - size: 13px;
    }
    
    .navbar - brand {
        font - size: 1.2rem;
    }
    
    .product - card {
        margin - bottom: 1rem;
    }
}

@media(max - width: 576px) {
    html {
        font - size: 12px;
    }
}

/* ===== Print Styles ===== */
@media print {
    .navbar, .footer, .btn, #quickSearchForm, .notification - dropdown {
        display: none!important;
    }
}